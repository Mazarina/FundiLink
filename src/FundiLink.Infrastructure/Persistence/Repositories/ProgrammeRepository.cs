using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

public class ProgrammeRepository : IProgrammeRepository
{
    private readonly FundiLinkDbContext _db;

    public ProgrammeRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<(IEnumerable<Programme> Items, int Total)> SearchAsync(
        string? keyword, InstitutionType? type, string? province, int page, int pageSize, CancellationToken ct)
    {
        var query = _db.Programmes
            .Include(p => p.Institution)
            .Where(p => p.DeletedAt == null && p.IsActive && p.Institution.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = keyword.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(k) ||
                p.Institution.Name.ToLower().Contains(k));
        }

        if (type.HasValue)
            query = query.Where(p => p.Institution.InstitutionType == type.Value);

        if (!string.IsNullOrWhiteSpace(province))
            query = query.Where(p => p.Institution.Province == province);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(p => p.Institution.Name)
            .ThenBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<Programme?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.Programmes
            .Include(p => p.Institution)
            .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null, ct);

    public async Task<IEnumerable<(Programme Programme, string InstitutionName)>> GetAllWithInstitutionAsync(CancellationToken ct)
    {
        var programmes = await _db.Programmes
            .Include(p => p.Institution)
            .Where(p => p.DeletedAt == null && p.IsActive && p.Institution.IsActive)
            .ToListAsync(ct);

        return programmes.Select(p => (p, p.Institution.Name));
    }

    public async Task AddAsync(Programme programme, CancellationToken ct)
        => await _db.Programmes.AddAsync(programme, ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
