using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

public class CareerRepository : ICareerRepository
{
    private readonly FundiLinkDbContext _db;

    public CareerRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<CareerOpportunity>> GetActiveAsync(
        string? fieldOfInterest, string? province, CareerOpportunityType? opportunityType, CancellationToken ct)
    {
        var query = _db.CareerOpportunities
            .Where(o => o.IsActive)
            .AsQueryable();

        if (opportunityType.HasValue)
            query = query.Where(o => o.OpportunityType == opportunityType.Value);

        var opportunities = await query.OrderBy(o => o.Title).ToListAsync(ct);

        // Field of interest and province are stored as JSON; filter in memory after the query.
        if (!string.IsNullOrWhiteSpace(fieldOfInterest))
        {
            var f = fieldOfInterest.Trim();
            opportunities = opportunities
                .Where(o => o.FieldsOfInterest.Count == 0 ||
                            o.FieldsOfInterest.Any(x => x.Contains(f, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(province))
        {
            var p = province.Trim();
            opportunities = opportunities
                .Where(o => o.ProvincesEligible.Count == 0 ||
                            o.ProvincesEligible.Any(x => string.Equals(x, p, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        return opportunities;
    }

    public async Task<IEnumerable<CareerOpportunity>> GetAllActiveAsync(CancellationToken ct)
        => await _db.CareerOpportunities
            .Where(o => o.IsActive)
            .OrderBy(o => o.Title)
            .ToListAsync(ct);

    public async Task<CareerOpportunity?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.CareerOpportunities.FirstOrDefaultAsync(o => o.Id == id, ct);
}
