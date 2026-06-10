using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Career.Queries.GetCareerMatches;

public class GetCareerMatchesHandler : IRequestHandler<GetCareerMatchesQuery, IEnumerable<CareerMatchDto>>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly ICareerRepository _careerRepository;

    public GetCareerMatchesHandler(
        ILearnerRepository learnerRepository,
        ICareerRepository careerRepository)
    {
        _learnerRepository = learnerRepository;
        _careerRepository = careerRepository;
    }

    // DISCLAIMER: Eligibility hints are guidance only ("you may be eligible"). They never
    // guarantee selection nor submit an application. Learners must apply on the provider's channel.
    public async Task<IEnumerable<CareerMatchDto>> Handle(GetCareerMatchesQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var opportunities = await _careerRepository.GetAllActiveAsync(cancellationToken);

        var matches = new List<CareerMatchDto>();
        foreach (var opportunity in opportunities)
        {
            if (!IsMatch(opportunity, learner.GradeLevel, learner.Province, out var reasons))
                continue;

            matches.Add(new CareerMatchDto(
                opportunity.Id,
                opportunity.Title,
                opportunity.ProviderName,
                opportunity.OpportunityType,
                opportunity.MinimumGradeLevel,
                opportunity.ApplicationCloseDate,
                opportunity.ExternalApplicationUrl,
                reasons,
                GuidanceOnly: true,
                CareerDisclaimer.Text));
        }

        return matches;
    }

    internal static bool IsMatch(CareerOpportunity opportunity, GradeLevel learnerGrade, string learnerProvince, out List<string> reasons)
    {
        reasons = [];

        // Grade gate: matches when no minimum is set, or the learner's grade meets/exceeds it.
        // Enum order (Grade11 < Grade12 < PostMatric) reflects increasing seniority.
        if (opportunity.MinimumGradeLevel is null)
        {
            reasons.Add("No minimum grade level required.");
        }
        else if (learnerGrade >= opportunity.MinimumGradeLevel.Value)
        {
            reasons.Add($"Your grade level ({learnerGrade}) meets the minimum of {opportunity.MinimumGradeLevel.Value}.");
        }
        else
        {
            return false;
        }

        // Province gate: matches when no provinces are listed, or the learner's province is included.
        if (opportunity.ProvincesEligible.Count == 0)
        {
            reasons.Add("Open to all provinces.");
        }
        else if (opportunity.ProvincesEligible.Any(p => string.Equals(p, learnerProvince, StringComparison.OrdinalIgnoreCase)))
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
