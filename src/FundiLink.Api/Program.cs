using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using FundiLink.Api.Health;
using FundiLink.Api.Middleware;
using FundiLink.Application;
using FundiLink.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Services ────────────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support — only ever wired up in Development (see pipeline below).
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FundiLink API",
        Version = "v1",
        Description = "FundiLink SmartApply — South African student opportunity platform"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Authentication — issuer, audience, lifetime and signing key are all validated.
// The signing key and issuer/audience values are configuration-driven (env vars in
// production); no unsigned or unvalidated tokens are accepted.
var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"]
    ?? throw new InvalidOperationException("JwtSettings:Issuer is not configured.");
var jwtAudience = builder.Configuration["JwtSettings:Audience"]
    ?? throw new InvalidOperationException("JwtSettings:Audience is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS — allowed origins come from configuration (Cors:AllowedOrigins, env var
// Cors__AllowedOrigins, comma-separated). In Development we additionally allow the
// local Vite dev server origins. Production must NOT fall back to localhost.
var configuredOrigins = (builder.Configuration["Cors:AllowedOrigins"] ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

var allowedOrigins = builder.Environment.IsDevelopment()
    ? configuredOrigins.Concat(new[] { "http://localhost:5173", "http://localhost:3000" }).Distinct().ToArray()
    : configuredOrigins;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AppCors", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

// Rate limiting — a global fixed-window limiter protects all endpoints from abuse.
// Limits are conservative defaults suitable for a small MVP deployment and can be
// tuned via configuration without code changes.
var rateLimitPermitLimit = builder.Configuration.GetValue("RateLimiting:PermitLimit", 100);
var rateLimitWindowSeconds = builder.Configuration.GetValue("RateLimiting:WindowSeconds", 60);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = rateLimitPermitLimit,
                Window = TimeSpan.FromSeconds(rateLimitWindowSeconds),
                QueueLimit = 0
            }));
});

// Health checks — liveness (/health) and readiness including database connectivity (/health/db)
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

// Forward proxy headers from Nginx (X-Forwarded-For / X-Forwarded-Proto) so
// UseHttpsRedirection and request scheme/IP reporting work correctly behind a reverse proxy.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// ── Pipeline ─────────────────────────────────────────────────────
var app = builder.Build();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FundiLink API v1");
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseCors("AppCors");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health/db");

// Apply pending EF Core migrations on startup. This is idempotent and never
// drops or recreates the schema — it only applies migrations that have not
// yet been applied. Skipped for non-relational providers (e.g. the InMemory
// provider used in integration tests).
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FundiLink.Infrastructure.Persistence.FundiLinkDbContext>();
    if (dbContext.Database.IsRelational())
    {
        await dbContext.Database.MigrateAsync();
    }
}

// Seed roles on startup (idempotent)
await FundiLink.Infrastructure.Persistence.Seed.RoleSeeder.SeedRolesAsync(app.Services);

// Seed sample programmes for guidance only (runs once, when no institutions exist)
await FundiLink.Infrastructure.Persistence.Seed.ProgrammeSeedData.SeedAsync(app.Services);

// Seed curated public bursary examples for guidance only (runs once, when no bursaries exist)
await FundiLink.Infrastructure.Persistence.Seed.BursarySeedData.SeedAsync(app.Services);

// Seed curated accommodation & early-career examples for guidance only (runs once, when empty)
await FundiLink.Infrastructure.Persistence.Seed.AccommodationSeedData.SeedAsync(app.Services);
await FundiLink.Infrastructure.Persistence.Seed.CareerSeedData.SeedAsync(app.Services);

app.Run();

// Required for WebApplicationFactory in integration tests
public partial class Program { }
