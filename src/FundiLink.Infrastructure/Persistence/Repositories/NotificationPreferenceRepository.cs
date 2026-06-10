using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

public class NotificationPreferenceRepository : INotificationPreferenceRepository
{
    private readonly FundiLinkDbContext _db;

    public NotificationPreferenceRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<NotificationPreference?> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct)
        => await _db.NotificationPreferences
            .FirstOrDefaultAsync(p => p.LearnerId == learnerId && p.DeletedAt == null, ct);

    public async Task AddAsync(NotificationPreference preference, CancellationToken ct)
        => await _db.NotificationPreferences.AddAsync(preference, ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
