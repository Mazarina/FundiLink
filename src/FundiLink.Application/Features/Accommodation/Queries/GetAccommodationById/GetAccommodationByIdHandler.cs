using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Accommodation.Queries.GetAccommodationListings;
using MediatR;

namespace FundiLink.Application.Features.Accommodation.Queries.GetAccommodationById;

public class GetAccommodationByIdHandler : IRequestHandler<GetAccommodationByIdQuery, AccommodationListingDto?>
{
    private readonly IAccommodationRepository _accommodationRepository;

    public GetAccommodationByIdHandler(IAccommodationRepository accommodationRepository)
    {
        _accommodationRepository = accommodationRepository;
    }

    public async Task<AccommodationListingDto?> Handle(GetAccommodationByIdQuery request, CancellationToken cancellationToken)
    {
        var listing = await _accommodationRepository.GetByIdAsync(request.Id, cancellationToken);
        return listing is null ? null : GetAccommodationListingsHandler.ToDto(listing);
    }
}
