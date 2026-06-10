using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

public class BursaryApplicationRepository : IBursaryApplicationRepository
{
    private readonly FundiLinkDbContext _db;

    public BursaryApplicationRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<BursaryApplication>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct)
        => await _db.BursaryApplications
            .Include(a => a.Bursary)
            .Where(a => a.LearnerId == learnerId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(ct);

    public async Task<BursaryApplication?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.BursaryApplications
            .Include(a => a.Bursary)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task AddAsync(BursaryApplication application, CancellationToken ct)
        => await _db.BursaryApplications.AddAsync(application, ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
