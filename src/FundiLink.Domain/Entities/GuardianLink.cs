using FundiLink.Domain.Common;

namespace FundiLink.Domain.Entities;

/// <summary>
/// Links a guardian (an authenticated user) to a minor learner for consent-gated,
/// read-only co-access. The link alone grants nothing — access is always gated by a
/// current GuardianCoAccess consent and limited to its consented scope (data minimisation).
/// </summary>
public class GuardianLink : BaseEntity
{
    public Guid LearnerId { get; private set; }

    /// <summary>The guardian's authenticated user id (identity user).</summary>
    public string GuardianUserId { get; private set; } = default!;

    public string GuardianName { get; private set; } = default!;
    public string GuardianContact { get; private set; } = default!;

    private GuardianLink() { }

    public static GuardianLink Create(
        Guid learnerId,
        string guardianUserId,
        string guardianName,
        string guardianContact)
    {
        if (string.IsNullOrWhiteSpace(guardianUserId))
            throw new ArgumentException("Guardian user id is required.", nameof(guardianUserId));
        if (string.IsNullOrWhiteSpace(guardianName))
            throw new ArgumentException("Guardian name is required.", nameof(guardianName));

        return new GuardianLink
        {
            LearnerId = learnerId,
            GuardianUserId = guardianUserId.Trim(),
            GuardianName = guardianName.Trim(),
            GuardianContact = (guardianContact ?? string.Empty).Trim()
        };
    }
}
