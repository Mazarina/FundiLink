using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Applications.Queries.GetApplicationById;

public class GetApplicationByIdHandler : IRequestHandler<GetApplicationByIdQuery, ApplicationDetailDto>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IApplicationRepository _applicationRepository;

    public GetApplicationByIdHandler(
        ILearnerRepository learnerRepository,
        IApplicationRepository applicationRepository)
    {
        _learnerRepository = learnerRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<ApplicationDetailDto> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var application = await _applicationRepository.GetByIdAsync(request.ApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException("Application not found.");

        if (application.LearnerId != learner.Id)
            throw new UnauthorizedAccessException("You do not have access to this application.");

        return new ApplicationDetailDto(
            application.Id,
            application.ProgrammeId,
            application.Programme.Name,
            application.Programme.Institution.Name,
            application.Status,
            application.Notes,
            application.DeadlineDate,
            application.SubmittedAt,
            application.OutcomeAt
        );
    }
}
