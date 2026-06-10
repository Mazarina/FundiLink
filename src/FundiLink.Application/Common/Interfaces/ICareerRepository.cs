using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;

namespace FundiLink.Application.Common.Interfaces;

public interface ICareerRepository
{
    Task<IEnumerable<CareerOpportunity>> GetActiveAsync(
        string? fieldOfInterest, string? province, CareerOpportunityType? opportunityType, CancellationToken ct);

    Task<IEnumerable<CareerOpportunity>> GetAllActiveAsync(CancellationToken ct);

    Task<CareerOpportunity?> GetByIdAsync(Guid id, CancellationToken ct);
}
