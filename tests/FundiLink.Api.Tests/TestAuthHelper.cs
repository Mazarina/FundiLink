using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FundiLink.Api.Tests;

/// <summary>Shared helpers for registering/logging in test learners and authenticating an HttpClient.</summary>
public static class TestAuthHelper
{
    public const string Password = "P@ssw0rd123";

    public static object RegisterPayload(string email) => new
    {
        email,
        password = Password,
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

    /// <summary>Registers a new learner and returns an HttpClient authenticated as that learner.</summary>
    public static async Task<(HttpClient Client, string Email)> CreateAuthenticatedLearnerClientAsync(FundiLinkApiFactory factory)
    {
        var client = factory.CreateClient();
        var email = $"learner-{Guid.NewGuid():N}@example.com";

        var registerResponse = await client.PostAsJsonAsync("/api/v1/auth/register", RegisterPayload(email));
        registerResponse.EnsureSuccessStatusCode();

        var token = await LoginAsync(client, email);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return (client, email);
    }

    public static async Task<string> LoginAsync(HttpClient client, string email)
    {
        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password = Password });
        loginResponse.EnsureSuccessStatusCode();
        var body = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        return body!.AccessToken;
    }

    private record LoginResponse(string AccessToken, string RefreshToken, int ExpiresIn);
}
