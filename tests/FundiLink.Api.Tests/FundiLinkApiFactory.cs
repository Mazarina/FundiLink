using FundiLink.Infrastructure.Persistence;
using FundiLink.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace FundiLink.Api.Tests;

/// <summary>
/// Shared WebApplicationFactory for API integration tests: replaces the Npgsql-backed
/// FundiLinkDbContext with a fresh InMemory database per factory instance, and points
/// document storage at a temporary directory so file-based tests don't touch the real
/// document-store.
/// </summary>
public class FundiLinkApiFactory : WebApplicationFactory<Program>
{
    public string DocumentStoragePath { get; } =
        Path.Combine(Path.GetTempPath(), "fundilink-test-" + Guid.NewGuid());

    // Computed once: WebApplicationFactory invokes ConfigureWebHost/CreateHost more than
    // once while building the test server, so a Guid generated inline in the
    // UseInMemoryDatabase lambda would produce a different database on each call.
    private readonly string _databaseName = "ApiTestDb_" + Guid.NewGuid();

    static FundiLinkApiFactory()
    {
        // Program.cs reads JwtSettings:SecretKey/Issuer/Audience directly from
        // builder.Configuration before WebApplicationFactory's ConfigureAppConfiguration
        // override is merged in, so an in-memory config override here would arrive too
        // late and leave the JWT-issuing and JWT-validating sides using different keys.
        // Environment variables, by contrast, are picked up by CreateBuilder() itself,
        // so set them here instead.
        Environment.SetEnvironmentVariable("JwtSettings__SecretKey", "test-secret-key-that-is-long-enough-32ch");
        Environment.SetEnvironmentVariable("JwtSettings__Issuer", "FundiLink");
        Environment.SetEnvironmentVariable("JwtSettings__Audience", "FundiLink");
        Environment.SetEnvironmentVariable("JwtSettings__ExpiryMinutes", "60");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = "Host=localhost;Database=test;Username=test;Password=test",
                ["JwtSettings:SecretKey"] = "test-secret-key-that-is-long-enough-32ch",
                ["JwtSettings:Issuer"] = "FundiLink",
                ["JwtSettings:Audience"] = "FundiLink",
                ["JwtSettings:ExpiryMinutes"] = "60",
                ["DocumentStorage:RootPath"] = DocumentStoragePath,
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<FundiLinkDbContext>>();
            services.RemoveAll<FundiLinkDbContext>();
            services.AddDbContext<FundiLinkDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Program.cs seeds Identity roles after building the app, but that code path does
        // not run under WebApplicationFactory's host. Seed roles here instead so
        // registration/role-assignment work the same as in a real deployment.
        var host = base.CreateHost(builder);
        RoleSeeder.SeedRolesAsync(host.Services).GetAwaiter().GetResult();
        return host;
    }

    /// <summary>Assigns an additional Identity role to an already-registered user (test-only helper).</summary>
    public async Task AssignRoleAsync(string email, string role)
    {
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = await userManager.FindByEmailAsync(email)
            ?? throw new InvalidOperationException($"User '{email}' not found.");
        await userManager.AddToRoleAsync(user, role);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (Directory.Exists(DocumentStoragePath))
        {
            try { Directory.Delete(DocumentStoragePath, recursive: true); }
            catch { /* best-effort cleanup */ }
        }
    }
}
