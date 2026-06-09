using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Applications.Commands.UpdateApplicationStatus;

public class UpdateApplicationStatusHandler : IRequestHandler<UpdateApplicationStatusCommand>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IApplicationRepository _applicationRepository;

    public UpdateApplicationStatusHandler(
        ILearnerRepository learnerRepository,
        IApplicationRepository applicationRepository)
    {
        _learnerRepository = learnerRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task Handle(UpdateApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var application = await _applicationRepository.GetByIdAsync(request.ApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException("Application not found.");

        if (application.LearnerId != learner.Id)
            throw new UnauthorizedAccessException("You do not have access to this application.");

        application.UpdateStatus(request.NewStatus, request.Notes);
        await _applicationRepository.SaveChangesAsync(cancellationToken);
    }
}
