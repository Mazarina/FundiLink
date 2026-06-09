using MediatR;

namespace FundiLink.Application.Features.Documents.Commands.DeleteDocument;

public record DeleteDocumentCommand(Guid DocumentId, string UserId) : IRequest;
