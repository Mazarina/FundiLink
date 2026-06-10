using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Bursaries.Queries.GetBursaryMatches;

public class GetBursaryMatchesHandler : IRequestHandler<GetBursaryMatchesQuery, IEnumerable<BursaryMatchDto>>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IBursaryRepository _bursaryRepository;

    public GetBursaryMatchesHandler(
        ILearnerRepository learnerRepository,
        IBursaryRepository bursaryRepository)
    {
        _learnerRepository = learnerRepository;
        _bursaryRepository = bursaryRepository;
    }

    // DISCLAIMER: Matching is guidance only ("you may qualify"). It never guarantees
    // funding nor submits an application. Learners must apply on the funder's official portal.
    public async Task<IEnumerable<BursaryMatchDto>> Handle(GetBursaryMatchesQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var profile = await _learnerRepository.GetAcademicProfileByLearnerIdAsync(learner.Id, cancellationToken);
        var apsScore = profile?.ApsScore;

        var bursaries = await _bursaryRepository.GetAllActiveAsync(cancellationToken);

        var matches = new List<BursaryMatchDto>();
        foreach (var bursary in bursaries)
        {
            if (!IsMatch(bursary, apsScore, learner.Province, out var reasons))
                continue;

            matches.Add(new BursaryMatchDto(
                bursary.Id,
                bursary.Name,
                bursary.ProviderName,
                bursary.FundingType,
                bursary.MinimumAps,
                bursary.ExternalApplicationUrl,
                reasons,
                GuidanceOnly: true,
                BursaryDisclaimer.Text));
        }

        return matches;
    }

    internal static bool IsMatch(Bursary bursary, int? apsScore, string learnerProvince, out List<string> reasons)
    {
        reasons = [];

        // APS gate: matches when no minimum is set, or the learner meets/exceeds it.
        if (bursary.MinimumAps is null)
        {
            reasons.Add("No minimum APS required.");
        }
        else if (apsScore is not null && apsScore.Value >= bursary.MinimumAps.Value)
        {
            reasons.Add($"Your APS ({apsScore.Value}) meets the minimum of {bursary.MinimumAps.Value}.");
        }
        else
        {
            return false;
        }

        // Province gate: matches when no provinces are listed, or the learner's province is included.
        if (bursary.ProvincesEligible.Count == 0)
        {
            reasons.Add("Open to all provinces.");
        }
        else if (bursary.ProvincesEligible.Any(p => string.Equals(p, learnerProvince, StringComparison.OrdinalIgnoreCase)))
        {
            reasons.Add($"Available in your province ({learnerProvince}).");
        }
        else
        {
            return false;
        }

        return true;
    }
}
