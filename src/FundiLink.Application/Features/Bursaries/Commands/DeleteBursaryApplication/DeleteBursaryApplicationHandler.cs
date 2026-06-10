using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Bursaries.Commands.DeleteBursaryApplication;

public class DeleteBursaryApplicationHandler : IRequestHandler<DeleteBursaryApplicationCommand>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IBursaryApplicationRepository _bursaryApplicationRepository;

    public DeleteBursaryApplicationHandler(
        ILearnerRepository learnerRepository,
        IBursaryApplicationRepository bursaryApplicationRepository)
    {
        _learnerRepository = learnerRepository;
        _bursaryApplicationRepository = bursaryApplicationRepository;
    }

    public async Task Handle(DeleteBursaryApplicationCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var application = await _bursaryApplicationRepository.GetByIdAsync(request.BursaryApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException("Bursary application not found.");

        if (application.LearnerId != learner.Id)
            throw new UnauthorizedAccessException("You do not have access to this bursary application.");

        application.SoftDelete();
        await _bursaryApplicationRepository.SaveChangesAsync(cancellationToken);
    }
}
