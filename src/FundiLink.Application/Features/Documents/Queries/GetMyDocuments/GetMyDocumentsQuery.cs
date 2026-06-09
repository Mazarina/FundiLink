using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Documents.Queries.GetMyDocuments;

public record GetMyDocumentsQuery(string UserId) : IRequest<IEnumerable<DocumentDto>>;

public record DocumentDto(
    Guid Id,
    DocumentType DocumentType,
    string FileName,
    long SizeBytes,
    DocumentStatus Status,
    DateTime UploadedAt,
    string? RejectionReason);
