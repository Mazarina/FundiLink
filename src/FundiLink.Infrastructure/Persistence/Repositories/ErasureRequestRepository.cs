using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for learner-initiated erasure requests (POPIA right to erasure).
/// </summary>
public class ErasureRequestRepository : IErasureRequestRepository
{
    private readonly FundiLinkDbContext _db;

    public ErasureRequestRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(ErasureRequest request, CancellationToken ct)
        => await _db.ErasureRequests.AddAsync(request, ct);

    public async Task<ErasureRequest?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.ErasureRequests.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<ErasureRequest>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct)
        => await _db.ErasureRequests
            .Where(r => r.LearnerId == learnerId)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(ct);

    public async Task<ErasureRequest?> GetOpenByLearnerIdAsync(Guid learnerId, CancellationToken ct)
        => await _db.ErasureRequests
            .Where(r => r.LearnerId == learnerId
                && (r.Status == ErasureRequestStatus.Requested || r.Status == ErasureRequestStatus.Approved))
            .OrderByDescending(r => r.RequestedAt)
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<ErasureRequest>> GetPendingAsync(CancellationToken ct)
        => await _db.ErasureRequests
            .Where(r => r.Status == ErasureRequestStatus.Requested)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
