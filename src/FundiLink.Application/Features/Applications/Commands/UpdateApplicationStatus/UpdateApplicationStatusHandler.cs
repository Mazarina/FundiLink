using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Applications.Commands.UpdateApplicationStatus;

public class UpdateApplicationStatusHandler : IRequestHandler<UpdateApplicationStatusCommand>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly INotificationService _notificationService;

    public UpdateApplicationStatusHandler(
        ILearnerRepository learnerRepository,
        IApplicationRepository applicationRepository,
        INotificationService notificationService)
    {
        _learnerRepository = learnerRepository;
        _applicationRepository = applicationRepository;
        _notificationService = notificationService;
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

        await _notificationService.NotifyAsync(
            learner.Id,
            NotificationType.ApplicationStatusChange,
            "Application status updated",
            $"Your application status changed to {request.NewStatus}.",
            cancellationToken);
    }
}
