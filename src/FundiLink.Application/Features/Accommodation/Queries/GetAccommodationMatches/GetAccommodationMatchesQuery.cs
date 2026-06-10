using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Accommodation.Queries.GetAccommodationMatches;

public record GetAccommodationMatchesQuery(string UserId) : IRequest<IEnumerable<AccommodationMatchDto>>;

public record AccommodationMatchDto(
    Guid Id,
    string Name,
    string ProviderName,
    AccommodationType AccommodationType,
    string Province,
    string City,
    string? NearInstitution,
    decimal? IndicativeMonthlyCost,
    string? ContactUrl,
    IReadOnlyList<string> Reasons,
    bool GuidanceOnly,
    string Disclaimer
);
