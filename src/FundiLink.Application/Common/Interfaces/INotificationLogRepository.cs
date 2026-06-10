using FundiLink.Domain.Entities;

namespace FundiLink.Application.Common.Interfaces;

public interface INotificationLogRepository
{
    // Append-only — no update or delete operations are exposed.
    Task AddAsync(NotificationLog log, CancellationToken ct);
    Task<IEnumerable<NotificationLog>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
