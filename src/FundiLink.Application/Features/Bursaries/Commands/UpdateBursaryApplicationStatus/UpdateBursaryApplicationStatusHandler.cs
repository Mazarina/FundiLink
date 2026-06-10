using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Bursaries.Commands.UpdateBursaryApplicationStatus;

public class UpdateBursaryApplicationStatusHandler : IRequestHandler<UpdateBursaryApplicationStatusCommand>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IBursaryApplicationRepository _bursaryApplicationRepository;
    private readonly INotificationService _notificationService;

    public UpdateBursaryApplicationStatusHandler(
        ILearnerRepository learnerRepository,
        IBursaryApplicationRepository bursaryApplicationRepository,
        INotificationService notificationService)
    {
        _learnerRepository = learnerRepository;
        _bursaryApplicationRepository = bursaryApplicationRepository;
        _notificationService = notificationService;
    }

    public async Task Handle(UpdateBursaryApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var application = await _bursaryApplicationRepository.GetByIdAsync(request.BursaryApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException("Bursary application not found.");

        if (application.LearnerId != learner.Id)
            throw new UnauthorizedAccessException("You do not have access to this bursary application.");

        application.UpdateStatus(request.NewStatus, request.Notes);
        await _bursaryApplicationRepository.SaveChangesAsync(cancellationToken);

        await _notificationService.NotifyAsync(
            learner.Id,
            NotificationType.BursaryStatusChange,
            "Bursary application status updated",
            $"Your bursary application status changed to {request.NewStatus}.",
            cancellationToken);
    }
}
