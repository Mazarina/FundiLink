using FundiLink.Application.Common.Models;

namespace FundiLink.Application.Common.Interfaces;

/// <summary>
/// Deterministic, in-process generator of deadline-reminder notifications. Given the
/// current date it finds upcoming programme/bursary application deadlines (within a
/// configurable day window) for active learners and dispatches guidance reminders via
/// <see cref="INotificationService"/> (which honours notification preferences and writes
/// append-only logs). Idempotent: at most one deadline reminder per learner per UTC day.
/// Reminders are guidance only — FundiLink is not an official admissions/funding portal.
/// No external scheduler is wired in this phase.
/// </summary>
public interface IDeadlineReminderService
{
    Task<ReminderRunResult> GenerateRemindersAsync(
        DateTime nowUtc, int windowDays, CancellationToken ct);
}
