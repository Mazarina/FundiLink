using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;

namespace FundiLink.Application.Common.Interfaces;

/// <summary>
/// Append-only guardian consent repository. Deliberately exposes no update or delete
/// methods — grants and revocations are both appended, preserving full consent history.
/// </summary>
public interface IGuardianConsentRepository
{
    Task AddAsync(GuardianConsent consent, CancellationToken ct);

    /// <summary>Full append-only history for a learner, newest first.</summary>
    Task<IReadOnlyList<GuardianConsent>> GetHistoryByLearnerIdAsync(Guid learnerId, CancellationToken ct);

    /// <summary>The most recent record for a learner and consent type, or null if none.</summary>
    Task<GuardianConsent?> GetLatestAsync(Guid learnerId, ConsentType consentType, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}
