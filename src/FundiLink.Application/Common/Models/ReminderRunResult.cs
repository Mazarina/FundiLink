namespace FundiLink.Application.Common.Models;

/// <summary>
/// Aggregate, non-PII outcome of a deadline-reminder pass. Counts only — no learner
/// personal information is surfaced.
/// </summary>
public record ReminderRunResult(
    int LearnersWithUpcomingDeadlines,
    int RemindersSent,
    int RemindersSkippedAlreadySent);
