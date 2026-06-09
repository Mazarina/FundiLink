using FundiLink.Domain.Entities;

namespace FundiLink.Application.Common.Interfaces;

/// <summary>
/// Append-only audit log repository. Deliberately exposes no update or delete methods.
/// </summary>
public interface IAuditLogRepository
{
    Task AddAsync(AuditLogEntry entry, CancellationToken ct);
    Task<(IEnumerable<AuditLogEntry> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
