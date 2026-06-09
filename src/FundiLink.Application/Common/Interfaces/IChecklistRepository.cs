using FundiLink.Domain.Entities;

namespace FundiLink.Application.Common.Interfaces;

public interface IChecklistRepository
{
    Task<IEnumerable<DocumentChecklistItem>> GetByApplicationIdAsync(Guid applicationId, CancellationToken ct);
    Task<DocumentChecklistItem?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(DocumentChecklistItem item, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
