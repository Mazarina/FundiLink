using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;

namespace FundiLink.Application.Common.Interfaces;

public interface INotificationLogRepository
{
    // Append-only — no update or delete operations are exposed.
    Task AddAsync(NotificationLog log, CancellationToken ct);
    Task<IEnumerable<NotificationLog>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct);

    /// <summary>
    /// True when a log of the given type already exists for the learner with a
    /// <c>SentAt</c> on the supplied UTC calendar date. Underpins idempotent reminder runs
    /// (at most one deadline reminder per learner per day).
    /// </summary>
    Task<bool> HasLogForTypeOnDateAsync(
        Guid learnerId, NotificationType type, DateTime utcDate, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}
