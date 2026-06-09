using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;

namespace FundiLink.Application.Common.Interfaces;

public interface IProgrammeRepository
{
    Task<(IEnumerable<Programme> Items, int Total)> SearchAsync(
        string? keyword, InstitutionType? type, string? province, int page, int pageSize, CancellationToken ct);

    Task<Programme?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<IEnumerable<(Programme Programme, string InstitutionName)>> GetAllWithInstitutionAsync(CancellationToken ct);

    Task AddAsync(Programme programme, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
