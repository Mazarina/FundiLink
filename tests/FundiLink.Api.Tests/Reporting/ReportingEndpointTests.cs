using System.Net;
using FundiLink.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FundiLink.Api.Tests.Reporting;

/// <summary>
/// RBAC tests for the read-only reporting endpoints. Unauthenticated callers must be rejected
/// on every reporting route (dashboard, POPIA summary, audit activity report).
/// </summary>
public class ReportingEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ReportingEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Default"] = "Host=localhost;Database=test;Username=test;Password=test",
                    ["JwtSettings:SecretKey"] = "test-secret-key-that-is-long-enough-32ch",
                    ["JwtSettings:Issuer"] = "FundiLink",
                    ["JwtSettings:Audience"] = "FundiLink",
                    ["JwtSettings:ExpiryMinutes"] = "60"
                });
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<FundiLinkDbContext>>();
                services.RemoveAll<FundiLinkDbContext>();
                services.AddDbContext<FundiLinkDbContext>(options =>
                    options.UseInMemoryDatabase("ReportingTestDb_" + Guid.NewGuid()));
            });
        });
    }

    [Theory]
    [InlineData("/api/v1/reporting/dashboard")]
    [InlineData("/api/v1/reporting/popia-summary")]
    [InlineData("/api/v1/reporting/audit-activity")]
    public async Task ReportingEndpoints_RejectUnauthenticatedCallers(string url)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
