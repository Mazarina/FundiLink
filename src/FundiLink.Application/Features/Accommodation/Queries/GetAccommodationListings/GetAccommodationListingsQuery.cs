using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Accommodation.Queries.GetAccommodationListings;

public record GetAccommodationListingsQuery(
    string? Province,
    string? NearInstitution,
    AccommodationType? AccommodationType
) : IRequest<IEnumerable<AccommodationListingDto>>;

public record AccommodationListingDto(
    Guid Id,
    string Name,
    string ProviderName,
    string Description,
    AccommodationType AccommodationType,
    string Province,
    string City,
    string? NearInstitution,
    decimal? IndicativeMonthlyCost,
    string? ContactUrl,
    string Disclaimer
);
