using FundiLink.Application.Common.Models;

namespace FundiLink.Application.Common.Interfaces;

/// <summary>
/// Read-only, in-process query over upcoming application and bursary-application deadlines.
/// Deterministic: given a date window it returns the active (non-deleted) learner deadlines
/// that fall inside it. Surfaces only the minimal fields needed to compose a guidance
/// reminder — no broad learner personal information.
/// </summary>
public interface IDeadlineQueryRepository
{
    /// <summary>
    /// Returns upcoming deadlines for active learners with a deadline date on or after
    /// <paramref name="fromInclusive"/> and on or before <paramref name="toInclusive"/>.
    /// </summary>
    Task<IReadOnlyList<UpcomingDeadline>> GetUpcomingDeadlinesAsync(
        DateTime fromInclusive, DateTime toInclusive, CancellationToken ct);

    /// <summary>
    /// Owner-scoped variant: returns upcoming deadlines for the single supplied learner whose
    /// deadline date falls inside the window. Used by the learner home dashboard. Surfaces only
    /// the learner's own deadlines — no cross-learner data.
    /// </summary>
    Task<IReadOnlyList<UpcomingDeadline>> GetUpcomingDeadlinesForLearnerAsync(
        Guid learnerId, DateTime fromInclusive, DateTime toInclusive, CancellationToken ct);
}
