using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

/// <summary>
/// Append-only record of a guardian's consent for a minor learner.
/// A grant is one immutable record; a revocation is captured as a separate
/// append-only record (Status = Revoked) — original grants are never mutated or
/// deleted, preserving a full auditable consent history (POPIA right to withdraw).
/// POPIA-minimal: stores recorded guardian identity, consent type/scope, and timestamps.
/// </summary>
public class GuardianConsent
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LearnerId { get; private set; }
    public ConsentType ConsentType { get; private set; }
    public ConsentScope Scope { get; private set; }
    public ConsentStatus Status { get; private set; }

    // Recorded guardian identity at the time of the action (POPIA-minimal).
    public string GuardianName { get; private set; } = default!;
    public string GuardianContact { get; private set; } = default!;

    public DateTime RecordedAt { get; private set; }

    private GuardianConsent() { }

    /// <summary>Records a new consent grant for a minor learner. Immutable once created.</summary>
    public static GuardianConsent Grant(
        Guid learnerId,
        ConsentType consentType,
        ConsentScope scope,
        string guardianName,
        string guardianContact)
    {
        if (string.IsNullOrWhiteSpace(guardianName))
            throw new ArgumentException("Guardian name is required to record consent.", nameof(guardianName));
        if (string.IsNullOrWhiteSpace(guardianContact))
            throw new ArgumentException("Guardian contact is required to record consent.", nameof(guardianContact));

        return new GuardianConsent
        {
            Id = Guid.NewGuid(),
            LearnerId = learnerId,
            ConsentType = consentType,
            Scope = scope,
            Status = ConsentStatus.Granted,
            GuardianName = guardianName.Trim(),
            GuardianContact = guardianContact.Trim(),
            RecordedAt = DateTime.UtcNow
        };
    }

    /// <summary>Records a revocation as a new append-only record (right to withdraw).</summary>
    public static GuardianConsent Revoke(
        Guid learnerId,
        ConsentType consentType,
        ConsentScope scope,
        string guardianName,
        string guardianContact)
    {
        var record = Grant(learnerId, consentType, scope, guardianName, guardianContact);
        record.Status = ConsentStatus.Revoked;
        return record;
    }
}
