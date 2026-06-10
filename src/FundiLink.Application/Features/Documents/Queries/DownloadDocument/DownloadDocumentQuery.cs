using MediatR;

namespace FundiLink.Application.Features.Documents.Queries.DownloadDocument;

public record DownloadDocumentQuery(
    Guid DocumentId,
    string UserId,
    bool IsAdminOrSupport = false,
    string? ActorRole = null) : IRequest<DownloadDocumentResult>;

public record DownloadDocumentResult(Stream Stream, string ContentType, string FileName);
