using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Career.Queries.GetMyCareerInterests;

public record GetMyCareerInterestsQuery(string UserId) : IRequest<IEnumerable<CareerInterestSummaryDto>>;

public record CareerInterestSummaryDto(
    Guid Id,
    Guid CareerOpportunityId,
    string OpportunityTitle,
    string ProviderName,
    OpportunityInterestStatus Status,
    string? Notes,
    string? ExternalApplicationUrl,
    string Disclaimer
);
