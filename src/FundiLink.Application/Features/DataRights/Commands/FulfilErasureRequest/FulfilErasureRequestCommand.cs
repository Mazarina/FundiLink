using MediatR;

namespace FundiLink.Application.Features.DataRights.Commands.FulfilErasureRequest;

/// <summary>
/// Admin fulfils an erasure request: anonymises/soft-deletes the learner's personal data
/// while preserving append-only audit and consent records. Deliberate, audited admin action.
/// RBAC enforced at the API boundary.
/// </summary>
public record FulfilErasureRequestCommand(
    Guid RequestId,
    string ActorUserId,
    string ActorRole,
    string? Note
) : IRequest;
