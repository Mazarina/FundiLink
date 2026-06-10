using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Career.Commands.UpdateCareerInterestStatus;

public class UpdateCareerInterestStatusHandler : IRequestHandler<UpdateCareerInterestStatusCommand>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly ICareerInterestRepository _interestRepository;

    public UpdateCareerInterestStatusHandler(
        ILearnerRepository learnerRepository,
        ICareerInterestRepository interestRepository)
    {
        _learnerRepository = learnerRepository;
        _interestRepository = interestRepository;
    }

    public async Task Handle(UpdateCareerInterestStatusCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var interest = await _interestRepository.GetByIdAsync(request.CareerInterestId, cancellationToken)
            ?? throw new KeyNotFoundException("Career interest not found.");

        if (interest.LearnerId != learner.Id)
            throw new UnauthorizedAccessException("You do not have access to this career interest.");

        interest.UpdateStatus(request.NewStatus, request.Notes);
        await _interestRepository.SaveChangesAsync(cancellationToken);
    }
}
