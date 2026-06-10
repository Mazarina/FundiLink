using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;

namespace FundiLink.Application.Common.Interfaces;

public interface IBursaryRepository
{
    Task<IEnumerable<Bursary>> GetActiveAsync(
        string? fieldOfStudy, string? province, BursaryFundingType? fundingType, CancellationToken ct);

    Task<IEnumerable<Bursary>> GetAllActiveAsync(CancellationToken ct);

    Task<Bursary?> GetByIdAsync(Guid id, CancellationToken ct);

    Task AddAsync(Bursary bursary, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
