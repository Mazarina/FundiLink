using FundiLink.Domain.Entities;

namespace FundiLink.Application.Common.Interfaces;

public interface IAccommodationInterestRepository
{
    Task<IEnumerable<AccommodationInterest>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct);
    Task<AccommodationInterest?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<AccommodationInterest?> GetByLearnerAndListingAsync(Guid learnerId, Guid listingId, CancellationToken ct);
    Task AddAsync(AccommodationInterest interest, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
