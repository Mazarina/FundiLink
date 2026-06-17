using System.Net;
using System.Net.Http.Json;

namespace FundiLink.Api.Tests.Auth;

public class AuthEndpointTests : IClassFixture<FundiLinkApiFactory>
{
    private readonly FundiLinkApiFactory _factory;

    public AuthEndpointTests(FundiLinkApiFactory factory)
    {
        _factory = factory;
    }

    private static object ValidRegisterPayload(string email) => new
    {
        email,
        password = "P@ssw0rd123",
        firstName = "Test",
        surname = "Learner",
        dateOfBirth = "2007-01-01",
        mobileNumber = "0820000000",
        province = "Gauteng",
        schoolName = "Test High School",
        schoolProvince = "Gauteng",
        gradeLevel = "Grade12",
        consentAccepted = true
    };

    [Fact]
    public async Task Register_WithValidData_ReturnsCreated()
    {
        var client = _factory.CreateClient();
        var email = $"register-{Guid.NewGuid():N}@example.com";

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", ValidRegisterPayload(email));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsAccessToken()
    {
        var client = _factory.CreateClient();
        var email = $"login-{Guid.NewGuid():N}@example.com";
        const string password = "P@ssw0rd123";

        var registerResponse = await client.PostAsJsonAsync("/api/v1/auth/register", ValidRegisterPayload(email));
        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        body!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var email = $"badlogin-{Guid.NewGuid():N}@example.com";

        var registerResponse = await client.PostAsJsonAsync("/api/v1/auth/register", ValidRegisterPayload(email));
        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password = "WrongPassword123!" });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/documents");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record LoginResponse(string AccessToken, string RefreshToken, int ExpiresIn);
}
