using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Consent.Commands.RecordConsent;

/// <summary>
/// Records (grants) a guardian consent for the caller's own minor learner profile.
/// Owner-scoped: the learner is resolved from the authenticated user id.
/// </summary>
public record RecordConsentCommand(
    string UserId,
    ConsentType ConsentType,
    ConsentScope Scope,
    string GuardianName,
    string GuardianContact
) : IRequest<Guid>;
