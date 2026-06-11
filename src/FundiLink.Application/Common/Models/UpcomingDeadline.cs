namespace FundiLink.Application.Common.Models;

/// <summary>
/// Kind of opportunity a deadline belongs to.
/// </summary>
public enum DeadlineKind
{
    ProgrammeApplication,
    BursaryApplication
}

/// <summary>
/// A minimal, typed view of a single upcoming deadline for an active learner.
/// Used by the deterministic reminder service to compose guidance reminders.
/// </summary>
public record UpcomingDeadline(
    Guid LearnerId,
    DeadlineKind Kind,
    string OpportunityName,
    DateTime DeadlineDate);
