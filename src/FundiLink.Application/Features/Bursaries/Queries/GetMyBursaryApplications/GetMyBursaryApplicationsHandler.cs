using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Bursaries.Queries.GetMyBursaryApplications;

public class GetMyBursaryApplicationsHandler : IRequestHandler<GetMyBursaryApplicationsQuery, IEnumerable<BursaryApplicationSummaryDto>>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IBursaryApplicationRepository _bursaryApplicationRepository;

    public GetMyBursaryApplicationsHandler(
        ILearnerRepository learnerRepository,
        IBursaryApplicationRepository bursaryApplicationRepository)
    {
        _learnerRepository = learnerRepository;
        _bursaryApplicationRepository = bursaryApplicationRepository;
    }

    public async Task<IEnumerable<BursaryApplicationSummaryDto>> Handle(GetMyBursaryApplicationsQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var applications = await _bursaryApplicationRepository.GetByLearnerIdAsync(learner.Id, cancellationToken);

        return applications.Select(a => new BursaryApplicationSummaryDto(
            a.Id,
            a.BursaryId,
            a.Bursary.Name,
            a.Bursary.ProviderName,
            a.Status,
            a.Notes,
            a.DeadlineDate,
            a.Bursary.ExternalApplicationUrl,
            BursaryDisclaimer.Text
        )).ToList();
    }
}
