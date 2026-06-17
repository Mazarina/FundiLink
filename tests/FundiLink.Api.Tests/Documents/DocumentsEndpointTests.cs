using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FundiLink.Api.Tests.Documents;

public class DocumentsEndpointTests : IClassFixture<FundiLinkApiFactory>
{
    private readonly FundiLinkApiFactory _factory;

    public DocumentsEndpointTests(FundiLinkApiFactory factory)
    {
        _factory = factory;
    }

    private static MultipartFormDataContent BuildUploadContent(
        byte[] bytes, string fileName, string contentType, string documentType = "IdDocument")
    {
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", fileName);
        content.Add(new StringContent(documentType), "documentType");
        return content;
    }

    [Fact]
    public async Task Upload_WithValidPdf_ReturnsCreated()
    {
        var (client, _) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);

        var response = await client.PostAsync("/api/v1/documents", BuildUploadContent(
            Encoding.UTF8.GetBytes("%PDF-1.4 test content"), "id.pdf", "application/pdf"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Upload_WithDisallowedFileType_ReturnsBadRequest()
    {
        var (client, _) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);

        var response = await client.PostAsync("/api/v1/documents", BuildUploadContent(
            Encoding.UTF8.GetBytes("#!/bin/sh\necho hi"), "script.sh", "application/x-sh"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Upload_OversizedFile_ReturnsBadRequest()
    {
        var (client, _) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);

        var oversized = new byte[10 * 1024 * 1024 + 1];
        var response = await client.PostAsync("/api/v1/documents", BuildUploadContent(
            oversized, "big.pdf", "application/pdf"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Download_OwnDocument_ReturnsFile()
    {
        var (client, _) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);
        var documentId = await UploadDocumentAsync(client, "id.pdf", "application/pdf", "%PDF-1.4 content");

        var response = await client.GetAsync($"/api/v1/documents/{documentId}/download");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Download_AnotherUsersDocument_ReturnsForbiddenOrUnauthorized()
    {
        var (ownerClient, _) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);
        var documentId = await UploadDocumentAsync(ownerClient, "id.pdf", "application/pdf", "%PDF-1.4 content");

        var (otherClient, _) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);

        var response = await otherClient.GetAsync($"/api/v1/documents/{documentId}/download");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Replace_OwnDocument_ReturnsNoContent()
    {
        var (client, _) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);
        var documentId = await UploadDocumentAsync(client, "id.pdf", "application/pdf", "%PDF-1.4 original");

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/documents/{documentId}")
        {
            Content = BuildUploadContent(Encoding.UTF8.GetBytes("%PDF-1.4 replacement"), "id-v2.pdf", "application/pdf")
        };
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Replace_AnotherUsersDocument_ReturnsUnauthorized()
    {
        var (ownerClient, _) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);
        var documentId = await UploadDocumentAsync(ownerClient, "id.pdf", "application/pdf", "%PDF-1.4 original");

        var (otherClient, _) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/documents/{documentId}")
        {
            Content = BuildUploadContent(Encoding.UTF8.GetBytes("%PDF-1.4 replacement"), "id-v2.pdf", "application/pdf")
        };
        var response = await otherClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_OwnDocument_ReturnsNoContent()
    {
        var (client, _) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);
        var documentId = await UploadDocumentAsync(client, "id.pdf", "application/pdf", "%PDF-1.4 content");

        var response = await client.DeleteAsync($"/api/v1/documents/{documentId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_AnotherUsersDocument_ReturnsUnauthorized()
    {
        var (ownerClient, _) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);
        var documentId = await UploadDocumentAsync(ownerClient, "id.pdf", "application/pdf", "%PDF-1.4 content");

        var (otherClient, _) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);

        var response = await otherClient.DeleteAsync($"/api/v1/documents/{documentId}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Download_WithPathTraversalId_ReturnsNotFound()
    {
        var (client, _) = await TestAuthHelper.CreateAuthenticatedLearnerClientAsync(_factory);

        // Not a valid Guid, so the route constraint rejects it before reaching the handler.
        var response = await client.GetAsync("/api/v1/documents/../../etc/passwd/download");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    private static async Task<Guid> UploadDocumentAsync(HttpClient client, string fileName, string contentType, string fileContent)
    {
        var response = await client.PostAsync("/api/v1/documents", BuildUploadContent(
            Encoding.UTF8.GetBytes(fileContent), fileName, contentType));
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("id").GetGuid();
    }
}
