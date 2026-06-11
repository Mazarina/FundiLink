using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Consent.Commands.RevokeConsent;

/// <summary>
/// Records a revocation (right to withdraw) of a previously granted consent for the
/// caller's own minor learner profile. Appended as a new record — history is preserved.
/// </summary>
public record RevokeConsentCommand(
    string UserId,
    ConsentType ConsentType
) : IRequest<Guid>;
