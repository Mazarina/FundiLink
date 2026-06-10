using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Career.Queries.GetMyCareerInterests;

public class GetMyCareerInterestsHandler
    : IRequestHandler<GetMyCareerInterestsQuery, IEnumerable<CareerInterestSummaryDto>>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly ICareerInterestRepository _interestRepository;

    public GetMyCareerInterestsHandler(
        ILearnerRepository learnerRepository,
        ICareerInterestRepository interestRepository)
    {
        _learnerRepository = learnerRepository;
        _interestRepository = interestRepository;
    }

    public async Task<IEnumerable<CareerInterestSummaryDto>> Handle(
        GetMyCareerInterestsQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var interests = await _interestRepository.GetByLearnerIdAsync(learner.Id, cancellationToken);

        return interests.Select(i => new CareerInterestSummaryDto(
            i.Id,
            i.CareerOpportunityId,
            i.CareerOpportunity.Title,
            i.CareerOpportunity.ProviderName,
            i.Status,
            i.Notes,
            i.CareerOpportunity.ExternalApplicationUrl,
            CareerDisclaimer.Text
        )).ToList();
    }
}
