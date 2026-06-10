using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Accommodation.Commands.UpdateAccommodationInterestStatus;

public class UpdateAccommodationInterestStatusHandler : IRequestHandler<UpdateAccommodationInterestStatusCommand>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IAccommodationInterestRepository _interestRepository;

    public UpdateAccommodationInterestStatusHandler(
        ILearnerRepository learnerRepository,
        IAccommodationInterestRepository interestRepository)
    {
        _learnerRepository = learnerRepository;
        _interestRepository = interestRepository;
    }

    public async Task Handle(UpdateAccommodationInterestStatusCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var interest = await _interestRepository.GetByIdAsync(request.AccommodationInterestId, cancellationToken)
            ?? throw new KeyNotFoundException("Accommodation interest not found.");

        if (interest.LearnerId != learner.Id)
            throw new UnauthorizedAccessException("You do not have access to this accommodation interest.");

        interest.UpdateStatus(request.NewStatus, request.Notes);
        await _interestRepository.SaveChangesAsync(cancellationToken);
    }
}
