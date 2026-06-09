using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.RejectDocument;

public record RejectDocumentCommand(
    string ActorUserId,
    string ActorRole,
    Guid DocumentId,
    string Reason) : IRequest;
