using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Career.Queries.GetCareerMatches;

public record GetCareerMatchesQuery(string UserId) : IRequest<IEnumerable<CareerMatchDto>>;

public record CareerMatchDto(
    Guid Id,
    string Title,
    string ProviderName,
    CareerOpportunityType OpportunityType,
    GradeLevel? MinimumGradeLevel,
    DateTime? ApplicationCloseDate,
    string? ExternalApplicationUrl,
    IReadOnlyList<string> Reasons,
    bool GuidanceOnly,
    string Disclaimer
);
