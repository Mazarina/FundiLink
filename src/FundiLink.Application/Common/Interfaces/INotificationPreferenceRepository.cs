using FundiLink.Domain.Entities;

namespace FundiLink.Application.Common.Interfaces;

public interface INotificationPreferenceRepository
{
    Task<NotificationPreference?> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct);
    Task AddAsync(NotificationPreference preference, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
