using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

public class BursaryRepository : IBursaryRepository
{
    private readonly FundiLinkDbContext _db;

    public BursaryRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Bursary>> GetActiveAsync(
        string? fieldOfStudy, string? province, BursaryFundingType? fundingType, CancellationToken ct)
    {
        var query = _db.Bursaries
            .Where(b => b.IsActive)
            .AsQueryable();

        if (fundingType.HasValue)
            query = query.Where(b => b.FundingType == fundingType.Value);

        var bursaries = await query
            .OrderBy(b => b.Name)
            .ToListAsync(ct);

        // Field of study and province are stored as JSON; filter in memory after the query.
        if (!string.IsNullOrWhiteSpace(fieldOfStudy))
        {
            var f = fieldOfStudy.Trim();
            bursaries = bursaries
                .Where(b => b.FieldsOfStudy.Count == 0 ||
                            b.FieldsOfStudy.Any(x => x.Contains(f, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(province))
        {
            var p = province.Trim();
            bursaries = bursaries
                .Where(b => b.ProvincesEligible.Count == 0 ||
                            b.ProvincesEligible.Any(x => string.Equals(x, p, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        return bursaries;
    }

    public async Task<IEnumerable<Bursary>> GetAllActiveAsync(CancellationToken ct)
        => await _db.Bursaries
            .Where(b => b.IsActive)
            .OrderBy(b => b.Name)
            .ToListAsync(ct);

    public async Task<Bursary?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.Bursaries.FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task AddAsync(Bursary bursary, CancellationToken ct)
        => await _db.Bursaries.AddAsync(bursary, ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
