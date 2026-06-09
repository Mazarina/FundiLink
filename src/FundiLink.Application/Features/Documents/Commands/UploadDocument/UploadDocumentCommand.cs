using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Documents.Commands.UploadDocument;

public record UploadDocumentCommand(
    string UserId,
    DocumentType DocumentType,
    string FileName,
    string ContentType,
    long SizeBytes,
    Stream Content
) : IRequest<Guid>;
