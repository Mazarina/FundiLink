using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Accommodation.Queries.GetAccommodationMatches;

public class GetAccommodationMatchesHandler
    : IRequestHandler<GetAccommodationMatchesQuery, IEnumerable<AccommodationMatchDto>>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IAccommodationRepository _accommodationRepository;

    public GetAccommodationMatchesHandler(
        ILearnerRepository learnerRepository,
        IAccommodationRepository accommodationRepository)
    {
        _learnerRepository = learnerRepository;
        _accommodationRepository = accommodationRepository;
    }

    // DISCLAIMER: "May suit you" guidance only — never a guarantee of availability, price,
    // or safety, and never a booking. Learners must verify directly with the provider.
    public async Task<IEnumerable<AccommodationMatchDto>> Handle(
        GetAccommodationMatchesQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var listings = await _accommodationRepository.GetAllActiveAsync(cancellationToken);

        var matches = new List<AccommodationMatchDto>();
        foreach (var listing in listings)
        {
            if (!IsMatch(listing, learner.Province, out var reasons))
                continue;

            matches.Add(new AccommodationMatchDto(
                listing.Id,
                listing.Name,
                listing.ProviderName,
                listing.AccommodationType,
                listing.Province,
                listing.City,
                listing.NearInstitution,
                listing.IndicativeMonthlyCost,
                listing.ContactUrl,
                reasons,
                GuidanceOnly: true,
                AccommodationDisclaimer.Text));
        }

        return matches;
    }

    // A listing "may suit" a learner when it is in their province. Province is the only
    // profile signal stored for a learner's location in this MVP.
    internal static bool IsMatch(AccommodationListing listing, string learnerProvince, out List<string> reasons)
    {
        reasons = [];

        if (string.Equals(listing.Province, learnerProvince, StringComparison.OrdinalIgnoreCase))
        {
            reasons.Add($"Located in your province ({learnerProvince}).");
            return true;
        }

        return false;
    }
}
