using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

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
                    ["JwtSettings:SecretKey"] = "test-secret-key-that-is-long-enough-for-hmac256",
                    ["JwtSettings:Issuer"] = "FundiLink",
                    ["JwtSettings:Audience"] = "FundiLink",
                    ["JwtSettings:ExpiryMinutes"] = "60"
                });
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
}
