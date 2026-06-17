using System.Net;

namespace FundiLink.Api.Tests.Reporting;

/// <summary>
/// RBAC tests for the admin reporting endpoints: a learner (Student role only) must be
/// rejected, while a user holding the required admin role is granted access.
/// </summary>
public class AdminReportingTests : IClassFixture<FundiLinkApiFactory>
{
    private readonly FundiLinkApiFactory _factory;

    public AdminReportingTests(FundiLinkApiFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/api/v1/reporting/dashboard")]
    [InlineData("/api/v1/reporting/popia-summary")]
    [InlineData("/api/v1/reporting/audit-activity")]
    public async Task ReportingEndpoints_AsLearner_ReturnsForbidden(string url)
    {
        var (client, _) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);

        var response = await client.GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("/api/v1/reporting/dashboard")]
    [InlineData("/api/v1/reporting/popia-summary")]
    [InlineData("/api/v1/reporting/audit-activity")]
    public async Task ReportingEndpoints_AsSuperAdmin_ReturnsOk(string url)
    {
        var (client, email) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);
        await _factory.AssignRoleAsync(email, "SuperAdmin");

        // Re-authenticate so the access token includes the newly-assigned role claim.
        var token = await TestAuthHelper.LoginAsync(_factory.CreateClient(), email);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("/api/v1/reporting/dashboard")]
    [InlineData("/api/v1/reporting/popia-summary")]
    public async Task ReportingEndpoints_AsSupportAgent_ReturnsOk(string url)
    {
        var (client, email) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);
        await _factory.AssignRoleAsync(email, "SupportAgent");

        var token = await TestAuthHelper.LoginAsync(_factory.CreateClient(), email);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AuditActivity_AsSupportAgent_ReturnsForbidden()
    {
        var (client, email) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);
        await _factory.AssignRoleAsync(email, "SupportAgent");

        var token = await TestAuthHelper.LoginAsync(_factory.CreateClient(), email);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/v1/reporting/audit-activity");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
