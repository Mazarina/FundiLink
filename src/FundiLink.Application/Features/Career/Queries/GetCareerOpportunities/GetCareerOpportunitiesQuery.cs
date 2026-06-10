using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Career.Queries.GetCareerOpportunities;

public record GetCareerOpportunitiesQuery(
    string? FieldOfInterest,
    string? Province,
    CareerOpportunityType? OpportunityType
) : IRequest<IEnumerable<CareerOpportunityDto>>;

public record CareerOpportunityDto(
    Guid Id,
    string Title,
    string ProviderName,
    string Description,
    CareerOpportunityType OpportunityType,
    IReadOnlyList<string> FieldsOfInterest,
    GradeLevel? MinimumGradeLevel,
    IReadOnlyList<string> ProvincesEligible,
    DateTime? ApplicationCloseDate,
    string? ExternalApplicationUrl,
    string Disclaimer
);
