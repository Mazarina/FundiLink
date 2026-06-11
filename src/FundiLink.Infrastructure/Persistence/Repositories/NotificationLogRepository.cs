using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

/// <summary>
/// Append-only notification log repository. No update or delete operations are exposed.
/// </summary>
public class NotificationLogRepository : INotificationLogRepository
{
    private readonly FundiLinkDbContext _db;

    public NotificationLogRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(NotificationLog log, CancellationToken ct)
        => await _db.NotificationLogs.AddAsync(log, ct);

    public async Task<IEnumerable<NotificationLog>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct)
        => await _db.NotificationLogs
            .Where(l => l.LearnerId == learnerId)
            .OrderByDescending(l => l.SentAt)
            .ToListAsync(ct);

    public async Task<bool> HasLogForTypeOnDateAsync(
        Guid learnerId, NotificationType type, DateTime utcDate, CancellationToken ct)
    {
        var dayStart = utcDate.Date;
        var dayEnd = dayStart.AddDays(1);
        return await _db.NotificationLogs.AnyAsync(
            l => l.LearnerId == learnerId
                 && l.NotificationType == type
                 && l.SentAt >= dayStart
                 && l.SentAt < dayEnd,
            ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
