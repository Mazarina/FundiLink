using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Accommodation.Commands.UpdateAccommodationInterestStatus;

public record UpdateAccommodationInterestStatusCommand(
    Guid AccommodationInterestId,
    string UserId,
    OpportunityInterestStatus NewStatus,
    string? Notes
) : IRequest;
