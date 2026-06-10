using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Career.Commands.TrackCareerInterest;

public record TrackCareerInterestCommand(
    Guid CareerOpportunityId,
    string UserId,
    OpportunityInterestStatus Status = OpportunityInterestStatus.Saved,
    string? Notes = null
) : IRequest<Guid>;
