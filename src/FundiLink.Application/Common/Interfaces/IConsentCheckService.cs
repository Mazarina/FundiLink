using FundiLink.Domain.Enums;

namespace FundiLink.Application.Common.Interfaces;

/// <summary>
/// Deterministic consent-check service. Decides whether a given consent type is
/// currently effective for a learner based purely on the latest append-only record
/// — no external identity-verification or e-signature provider call in this phase.
/// A real provider may be wired later behind this interface (key via env only).
/// </summary>
public interface IConsentCheckService
{
    /// <summary>True when the most recent record for the consent type is a current grant.</summary>
    Task<bool> HasCurrentConsentAsync(Guid learnerId, ConsentType consentType, CancellationToken ct);
}
