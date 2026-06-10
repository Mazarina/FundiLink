using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Career.Queries.GetCareerOpportunities;
using MediatR;

namespace FundiLink.Application.Features.Career.Queries.GetCareerOpportunityById;

public class GetCareerOpportunityByIdHandler : IRequestHandler<GetCareerOpportunityByIdQuery, CareerOpportunityDto?>
{
    private readonly ICareerRepository _careerRepository;

    public GetCareerOpportunityByIdHandler(ICareerRepository careerRepository)
    {
        _careerRepository = careerRepository;
    }

    public async Task<CareerOpportunityDto?> Handle(GetCareerOpportunityByIdQuery request, CancellationToken cancellationToken)
    {
        var opportunity = await _careerRepository.GetByIdAsync(request.Id, cancellationToken);
        return opportunity is null ? null : GetCareerOpportunitiesHandler.ToDto(opportunity);
    }
}
