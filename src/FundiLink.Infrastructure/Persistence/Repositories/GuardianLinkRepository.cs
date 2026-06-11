using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

public class GuardianLinkRepository : IGuardianLinkRepository
{
    private readonly FundiLinkDbContext _db;

    public GuardianLinkRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(GuardianLink link, CancellationToken ct)
        => await _db.GuardianLinks.AddAsync(link, ct);

    public async Task<GuardianLink?> GetByGuardianAndLearnerAsync(string guardianUserId, Guid learnerId, CancellationToken ct)
        => await _db.GuardianLinks
            .FirstOrDefaultAsync(l => l.GuardianUserId == guardianUserId && l.LearnerId == learnerId, ct);

    public async Task<IReadOnlyList<GuardianLink>> GetByGuardianUserIdAsync(string guardianUserId, CancellationToken ct)
        => await _db.GuardianLinks
            .Where(l => l.GuardianUserId == guardianUserId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
