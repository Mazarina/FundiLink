using MediatR;

namespace FundiLink.Application.Features.Consent.Commands.LinkGuardian;

/// <summary>
/// Links a guardian (an existing authenticated user) to the caller's own minor learner
/// profile for consent-gated read-only co-access. Owner-scoped: the learner is resolved
/// from the authenticated caller. The link alone grants nothing without a current consent.
/// </summary>
public record LinkGuardianCommand(
    string UserId,
    string GuardianUserId,
    string GuardianName,
    string GuardianContact
) : IRequest<Guid>;
