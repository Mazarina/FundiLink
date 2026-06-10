using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;

namespace FundiLink.Application.Common.Interfaces;

public interface IAccommodationRepository
{
    Task<IEnumerable<AccommodationListing>> GetActiveAsync(
        string? province, string? nearInstitution, AccommodationType? accommodationType, CancellationToken ct);

    Task<IEnumerable<AccommodationListing>> GetAllActiveAsync(CancellationToken ct);

    Task<AccommodationListing?> GetByIdAsync(Guid id, CancellationToken ct);
}
