using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Applications.Queries.GetMyApplications;

public class GetMyApplicationsHandler : IRequestHandler<GetMyApplicationsQuery, IEnumerable<ApplicationSummaryDto>>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IApplicationRepository _applicationRepository;

    public GetMyApplicationsHandler(
        ILearnerRepository learnerRepository,
        IApplicationRepository applicationRepository)
    {
        _learnerRepository = learnerRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<IEnumerable<ApplicationSummaryDto>> Handle(GetMyApplicationsQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var applications = await _applicationRepository.GetByLearnerIdAsync(learner.Id, cancellationToken);

        return applications.Select(a => new ApplicationSummaryDto(
            a.Id,
            a.ProgrammeId,
            a.Programme.Name,
            a.Programme.Institution.Name,
            a.Status,
            a.DeadlineDate,
            a.SubmittedAt
        )).ToList();
    }
}
