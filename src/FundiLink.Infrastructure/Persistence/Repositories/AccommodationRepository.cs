using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

public class AccommodationRepository : IAccommodationRepository
{
    private readonly FundiLinkDbContext _db;

    public AccommodationRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<AccommodationListing>> GetActiveAsync(
        string? province, string? nearInstitution, AccommodationType? accommodationType, CancellationToken ct)
    {
        var query = _db.AccommodationListings
            .Where(a => a.IsActive)
            .AsQueryable();

        if (accommodationType.HasValue)
            query = query.Where(a => a.AccommodationType == accommodationType.Value);

        if (!string.IsNullOrWhiteSpace(province))
        {
            var p = province.Trim();
            query = query.Where(a => a.Province.ToLower() == p.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(nearInstitution))
        {
            var inst = nearInstitution.Trim();
            query = query.Where(a => a.NearInstitution != null && EF.Functions.ILike(a.NearInstitution, $"%{inst}%"));
        }

        return await query.OrderBy(a => a.Name).ToListAsync(ct);
    }

    public async Task<IEnumerable<AccommodationListing>> GetAllActiveAsync(CancellationToken ct)
        => await _db.AccommodationListings
            .Where(a => a.IsActive)
            .OrderBy(a => a.Name)
            .ToListAsync(ct);

    public async Task<AccommodationListing?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.AccommodationListings.FirstOrDefaultAsync(a => a.Id == id, ct);
}
