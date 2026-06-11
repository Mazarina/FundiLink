using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

/// <summary>
/// Append-only guardian consent repository. No update or delete operations are exposed —
/// grants and revocations are both appended, preserving the full consent history.
/// </summary>
public class GuardianConsentRepository : IGuardianConsentRepository
{
    private readonly FundiLinkDbContext _db;

    public GuardianConsentRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(GuardianConsent consent, CancellationToken ct)
        => await _db.GuardianConsents.AddAsync(consent, ct);

    public async Task<IReadOnlyList<GuardianConsent>> GetHistoryByLearnerIdAsync(Guid learnerId, CancellationToken ct)
        => await _db.GuardianConsents
            .Where(c => c.LearnerId == learnerId)
            .OrderByDescending(c => c.RecordedAt)
            .ToListAsync(ct);

    public async Task<GuardianConsent?> GetLatestAsync(Guid learnerId, ConsentType consentType, CancellationToken ct)
        => await _db.GuardianConsents
            .Where(c => c.LearnerId == learnerId && c.ConsentType == consentType)
            .OrderByDescending(c => c.RecordedAt)
            .FirstOrDefaultAsync(ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
