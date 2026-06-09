using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.VerifyDocument;

public record VerifyDocumentCommand(
    string ActorUserId,
    string ActorRole,
    Guid DocumentId) : IRequest;
