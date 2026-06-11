using MediatR;

namespace FundiLink.Application.Features.DataRights.Commands.ReviewErasureRequest;

/// <summary>
/// Admin reviews a pending erasure request: approve (proceed to fulfilment) or reject.
/// RBAC enforced at the API boundary; every decision is append-only audit-logged.
/// </summary>
public record ReviewErasureRequestCommand(
    Guid RequestId,
    bool Approve,
    string ActorUserId,
    string ActorRole,
    string? Note
) : IRequest;
