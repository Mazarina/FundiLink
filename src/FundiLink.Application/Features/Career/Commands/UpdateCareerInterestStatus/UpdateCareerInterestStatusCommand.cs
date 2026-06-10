using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Career.Commands.UpdateCareerInterestStatus;

public record UpdateCareerInterestStatusCommand(
    Guid CareerInterestId,
    string UserId,
    OpportunityInterestStatus NewStatus,
    string? Notes
) : IRequest;
