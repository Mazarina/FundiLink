using MediatR;

namespace FundiLink.Application.Features.Documents.Queries.DownloadDocument;

public record DownloadDocumentQuery(Guid DocumentId, string UserId) : IRequest<DownloadDocumentResult>;

public record DownloadDocumentResult(Stream Stream, string ContentType, string FileName);
