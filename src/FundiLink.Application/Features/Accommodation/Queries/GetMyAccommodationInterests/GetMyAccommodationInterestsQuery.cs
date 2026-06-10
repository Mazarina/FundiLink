using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Accommodation.Queries.GetMyAccommodationInterests;

public record GetMyAccommodationInterestsQuery(string UserId) : IRequest<IEnumerable<AccommodationInterestSummaryDto>>;

public record AccommodationInterestSummaryDto(
    Guid Id,
    Guid AccommodationListingId,
    string ListingName,
    string ProviderName,
    OpportunityInterestStatus Status,
    string? Notes,
    string? ContactUrl,
    string Disclaimer
);
