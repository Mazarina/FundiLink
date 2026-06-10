using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Accommodation.Queries.GetAccommodationListings;

public class GetAccommodationListingsHandler
    : IRequestHandler<GetAccommodationListingsQuery, IEnumerable<AccommodationListingDto>>
{
    private readonly IAccommodationRepository _accommodationRepository;

    public GetAccommodationListingsHandler(IAccommodationRepository accommodationRepository)
    {
        _accommodationRepository = accommodationRepository;
    }

    public async Task<IEnumerable<AccommodationListingDto>> Handle(
        GetAccommodationListingsQuery request, CancellationToken cancellationToken)
    {
        var listings = await _accommodationRepository.GetActiveAsync(
            request.Province, request.NearInstitution, request.AccommodationType, cancellationToken);

        return listings.Select(ToDto).ToList();
    }

    internal static AccommodationListingDto ToDto(AccommodationListing l) => new(
        l.Id,
        l.Name,
        l.ProviderName,
        l.Description,
        l.AccommodationType,
        l.Province,
        l.City,
        l.NearInstitution,
        l.IndicativeMonthlyCost,
        l.ContactUrl,
        AccommodationDisclaimer.Text);
}
