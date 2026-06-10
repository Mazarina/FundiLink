using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

public class AccommodationInterestRepository : IAccommodationInterestRepository
{
    private readonly FundiLinkDbContext _db;

    public AccommodationInterestRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<AccommodationInterest>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct)
        => await _db.AccommodationInterests
            .Include(i => i.AccommodationListing)
            .Where(i => i.LearnerId == learnerId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(ct);

    public async Task<AccommodationInterest?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.AccommodationInterests
            .Include(i => i.AccommodationListing)
            .FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<AccommodationInterest?> GetByLearnerAndListingAsync(Guid learnerId, Guid listingId, CancellationToken ct)
        => await _db.AccommodationInterests
            .Include(i => i.AccommodationListing)
            .FirstOrDefaultAsync(i => i.LearnerId == learnerId && i.AccommodationListingId == listingId, ct);

    public async Task AddAsync(AccommodationInterest interest, CancellationToken ct)
        => await _db.AccommodationInterests.AddAsync(interest, ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
