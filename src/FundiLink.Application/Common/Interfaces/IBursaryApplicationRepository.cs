using FundiLink.Domain.Entities;

namespace FundiLink.Application.Common.Interfaces;

public interface IBursaryApplicationRepository
{
    Task<IEnumerable<BursaryApplication>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct);
    Task<BursaryApplication?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(BursaryApplication application, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
