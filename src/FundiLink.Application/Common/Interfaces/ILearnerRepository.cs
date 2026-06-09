using FundiLink.Domain.Entities;

namespace FundiLink.Application.Common.Interfaces;

public interface ILearnerRepository
{
    Task<Learner?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Learner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Learner learner, CancellationToken cancellationToken = default);
    Task<AcademicProfile?> GetAcademicProfileByLearnerIdAsync(Guid learnerId, CancellationToken cancellationToken = default);
    Task AddAcademicProfileAsync(AcademicProfile profile, CancellationToken cancellationToken = default);
}
