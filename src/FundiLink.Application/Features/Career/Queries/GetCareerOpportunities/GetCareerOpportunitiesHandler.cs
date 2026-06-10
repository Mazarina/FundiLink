using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Career.Queries.GetCareerOpportunities;

public class GetCareerOpportunitiesHandler
    : IRequestHandler<GetCareerOpportunitiesQuery, IEnumerable<CareerOpportunityDto>>
{
    private readonly ICareerRepository _careerRepository;

    public GetCareerOpportunitiesHandler(ICareerRepository careerRepository)
    {
        _careerRepository = careerRepository;
    }

    public async Task<IEnumerable<CareerOpportunityDto>> Handle(
        GetCareerOpportunitiesQuery request, CancellationToken cancellationToken)
    {
        var opportunities = await _careerRepository.GetActiveAsync(
            request.FieldOfInterest, request.Province, request.OpportunityType, cancellationToken);

        return opportunities.Select(ToDto).ToList();
    }

    internal static CareerOpportunityDto ToDto(CareerOpportunity o) => new(
        o.Id,
        o.Title,
        o.ProviderName,
        o.Description,
        o.OpportunityType,
        o.FieldsOfInterest,
        o.MinimumGradeLevel,
        o.ProvincesEligible,
        o.ApplicationCloseDate,
        o.ExternalApplicationUrl,
        CareerDisclaimer.Text);
}
