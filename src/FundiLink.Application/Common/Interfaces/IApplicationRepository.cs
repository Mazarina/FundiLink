using FundiLink.Domain.Entities;

namespace FundiLink.Application.Common.Interfaces;

public interface IApplicationRepository
{
    Task<IEnumerable<LearnerApplication>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct);
    Task<LearnerApplication?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(LearnerApplication application, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
