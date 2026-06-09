using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly FundiLinkDbContext _db;

    public ApplicationRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<LearnerApplication>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct)
        => await _db.LearnerApplications
            .Include(a => a.Programme)
                .ThenInclude(p => p.Institution)
            .Where(a => a.LearnerId == learnerId && a.DeletedAt == null)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(ct);

    public async Task<LearnerApplication?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.LearnerApplications
            .Include(a => a.Programme)
                .ThenInclude(p => p.Institution)
            .FirstOrDefaultAsync(a => a.Id == id && a.DeletedAt == null, ct);

    public async Task AddAsync(LearnerApplication application, CancellationToken ct)
        => await _db.LearnerApplications.AddAsync(application, ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
