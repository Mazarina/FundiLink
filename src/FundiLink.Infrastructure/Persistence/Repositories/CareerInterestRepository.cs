using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

public class CareerInterestRepository : ICareerInterestRepository
{
    private readonly FundiLinkDbContext _db;

    public CareerInterestRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<CareerInterest>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct)
        => await _db.CareerInterests
            .Include(i => i.CareerOpportunity)
            .Where(i => i.LearnerId == learnerId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(ct);

    public async Task<CareerInterest?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.CareerInterests
            .Include(i => i.CareerOpportunity)
            .FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<CareerInterest?> GetByLearnerAndOpportunityAsync(Guid learnerId, Guid opportunityId, CancellationToken ct)
        => await _db.CareerInterests
            .Include(i => i.CareerOpportunity)
            .FirstOrDefaultAsync(i => i.LearnerId == learnerId && i.CareerOpportunityId == opportunityId, ct);

    public async Task AddAsync(CareerInterest interest, CancellationToken ct)
        => await _db.CareerInterests.AddAsync(interest, ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
