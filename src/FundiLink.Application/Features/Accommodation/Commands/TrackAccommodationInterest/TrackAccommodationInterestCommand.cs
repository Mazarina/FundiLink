using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Accommodation.Commands.TrackAccommodationInterest;

public record TrackAccommodationInterestCommand(
    Guid AccommodationListingId,
    string UserId,
    OpportunityInterestStatus Status = OpportunityInterestStatus.Saved,
    string? Notes = null
) : IRequest<Guid>;
