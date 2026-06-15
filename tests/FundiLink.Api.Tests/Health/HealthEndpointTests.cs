using System.Net;
using FundiLink.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FundiLink.Api.Tests.Health;

public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
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
                // Replace Npgsql with InMemory DB for tests
                services.RemoveAll<DbContextOptions<FundiLinkDbContext>>();
                services.RemoveAll<FundiLinkDbContext>();
                services.AddDbContext<FundiLinkDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));
            });
        });
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HealthDbEndpoint_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health/db");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
