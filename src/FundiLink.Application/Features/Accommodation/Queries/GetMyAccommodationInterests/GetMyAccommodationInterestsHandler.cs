using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Accommodation.Queries.GetMyAccommodationInterests;

public class GetMyAccommodationInterestsHandler
    : IRequestHandler<GetMyAccommodationInterestsQuery, IEnumerable<AccommodationInterestSummaryDto>>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IAccommodationInterestRepository _interestRepository;

    public GetMyAccommodationInterestsHandler(
        ILearnerRepository learnerRepository,
        IAccommodationInterestRepository interestRepository)
    {
        _learnerRepository = learnerRepository;
        _interestRepository = interestRepository;
    }

    public async Task<IEnumerable<AccommodationInterestSummaryDto>> Handle(
        GetMyAccommodationInterestsQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var interests = await _interestRepository.GetByLearnerIdAsync(learner.Id, cancellationToken);

        return interests.Select(i => new AccommodationInterestSummaryDto(
            i.Id,
            i.AccommodationListingId,
            i.AccommodationListing.Name,
            i.AccommodationListing.ProviderName,
            i.Status,
            i.Notes,
            i.AccommodationListing.ContactUrl,
            AccommodationDisclaimer.Text
        )).ToList();
    }
}
