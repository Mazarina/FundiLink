using FundiLink.Domain.Entities;

namespace FundiLink.Application.Common.Interfaces;

public interface ICareerInterestRepository
{
    Task<IEnumerable<CareerInterest>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct);
    Task<CareerInterest?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<CareerInterest?> GetByLearnerAndOpportunityAsync(Guid learnerId, Guid opportunityId, CancellationToken ct);
    Task AddAsync(CareerInterest interest, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
