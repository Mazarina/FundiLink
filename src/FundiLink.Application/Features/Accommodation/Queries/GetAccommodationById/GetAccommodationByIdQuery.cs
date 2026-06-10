using FundiLink.Application.Features.Accommodation.Queries.GetAccommodationListings;
using MediatR;

namespace FundiLink.Application.Features.Accommodation.Queries.GetAccommodationById;

public record GetAccommodationByIdQuery(Guid Id) : IRequest<AccommodationListingDto?>;
