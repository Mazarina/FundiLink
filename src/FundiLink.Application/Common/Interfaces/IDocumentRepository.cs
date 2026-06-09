using FundiLink.Domain.Entities;

namespace FundiLink.Application.Common.Interfaces;

public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Document>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct);
    Task AddAsync(Document document, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
