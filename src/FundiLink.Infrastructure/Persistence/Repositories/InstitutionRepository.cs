using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

public class InstitutionRepository : IInstitutionRepository
{
    private readonly FundiLinkDbContext _db;

    public InstitutionRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<Institution?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.Institutions.FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null, ct);

    public async Task AddAsync(Institution institution, CancellationToken ct)
        => await _db.Institutions.AddAsync(institution, ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
