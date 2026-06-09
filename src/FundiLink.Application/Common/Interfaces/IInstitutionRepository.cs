using FundiLink.Domain.Entities;

namespace FundiLink.Application.Common.Interfaces;

public interface IInstitutionRepository
{
    Task<Institution?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Institution institution, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
