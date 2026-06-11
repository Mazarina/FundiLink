using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Consent.Queries.GetLinkedLearners;

/// <summary>
/// Lists the learners a guardian is linked to, with whether co-access consent is
/// currently in place. Minimised — only name and consent state, no profile detail.
/// </summary>
public class GetLinkedLearnersHandler
    : IRequestHandler<GetLinkedLearnersQuery, IReadOnlyList<LinkedLearnerDto>>
{
    private readonly IGuardianLinkRepository _linkRepository;
    private readonly IConsentCheckService _consentCheckService;
    private readonly ILearnerRepository _learnerRepository;

    public GetLinkedLearnersHandler(
        IGuardianLinkRepository linkRepository,
        IConsentCheckService consentCheckService,
        ILearnerRepository learnerRepository)
    {
        _linkRepository = linkRepository;
        _consentCheckService = consentCheckService;
        _learnerRepository = learnerRepository;
    }

    public async Task<IReadOnlyList<LinkedLearnerDto>> Handle(
        GetLinkedLearnersQuery request, CancellationToken cancellationToken)
    {
        var links = await _linkRepository.GetByGuardianUserIdAsync(request.GuardianUserId, cancellationToken);

        var result = new List<LinkedLearnerDto>();
        foreach (var link in links)
        {
            var learner = await _learnerRepository.GetByIdAsync(link.LearnerId, cancellationToken);
            if (learner is null) continue;

            var hasConsent = await _consentCheckService.HasCurrentConsentAsync(
                link.LearnerId, ConsentType.GuardianCoAccess, cancellationToken);

            result.Add(new LinkedLearnerDto(
                learner.Id, learner.FirstName, learner.Surname, hasConsent));
        }

        return result;
    }
}
