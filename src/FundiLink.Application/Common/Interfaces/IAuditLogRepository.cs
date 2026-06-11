using FundiLink.Domain.Entities;

namespace FundiLink.Application.Common.Interfaces;

/// <summary>
/// Append-only audit log repository. Deliberately exposes no update or delete methods.
/// </summary>
public interface IAuditLogRepository
{
    Task AddAsync(AuditLogEntry entry, CancellationToken ct);
    Task<(IEnumerable<AuditLogEntry> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct);

    /// <summary>
    /// Filtered, paged view over the append-only audit log for staff reporting.
    /// All filters are optional; null/blank values are ignored. Reads only — never mutates.
    /// </summary>
    Task<(IEnumerable<AuditLogEntry> Items, int Total)> GetFilteredAsync(
        string? action, string? actorRole, DateTime? fromUtc, DateTime? toUtc,
        int page, int pageSize, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}
