using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

/// <summary>
/// Append-only audit log repository. No update or delete operations are exposed.
/// </summary>
public class AuditLogRepository : IAuditLogRepository
{
    private readonly FundiLinkDbContext _db;

    public AuditLogRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(AuditLogEntry entry, CancellationToken ct)
        => await _db.AuditLogEntries.AddAsync(entry, ct);

    public async Task<(IEnumerable<AuditLogEntry> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = _db.AuditLogEntries.OrderByDescending(a => a.OccurredAt);
        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return (items, total);
    }

    public async Task<(IEnumerable<AuditLogEntry> Items, int Total)> GetFilteredAsync(
        string? action, string? actorRole, DateTime? fromUtc, DateTime? toUtc,
        int page, int pageSize, CancellationToken ct)
    {
        IQueryable<AuditLogEntry> query = _db.AuditLogEntries;

        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(a => a.Action == action);

        if (!string.IsNullOrWhiteSpace(actorRole))
            query = query.Where(a => a.ActorRole == actorRole);

        if (fromUtc.HasValue)
            query = query.Where(a => a.OccurredAt >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(a => a.OccurredAt <= toUtc.Value);

        var ordered = query.OrderByDescending(a => a.OccurredAt);
        var total = await ordered.CountAsync(ct);
        var items = await ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return (items, total);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
