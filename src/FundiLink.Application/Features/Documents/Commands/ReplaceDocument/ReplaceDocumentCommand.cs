using MediatR;

namespace FundiLink.Application.Features.Documents.Commands.ReplaceDocument;

public record ReplaceDocumentCommand(
    Guid DocumentId,
    string UserId,
    string FileName,
    string ContentType,
    long SizeBytes,
    Stream Content) : IRequest;
