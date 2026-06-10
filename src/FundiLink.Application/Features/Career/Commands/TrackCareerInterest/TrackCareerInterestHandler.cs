using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Career.Commands.TrackCareerInterest;

public class TrackCareerInterestHandler : IRequestHandler<TrackCareerInterestCommand, Guid>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly ICareerRepository _careerRepository;
    private readonly ICareerInterestRepository _interestRepository;

    public TrackCareerInterestHandler(
        ILearnerRepository learnerRepository,
        ICareerRepository careerRepository,
        ICareerInterestRepository interestRepository)
    {
        _learnerRepository = learnerRepository;
        _careerRepository = careerRepository;
        _interestRepository = interestRepository;
    }

    public async Task<Guid> Handle(TrackCareerInterestCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var opportunity = await _careerRepository.GetByIdAsync(request.CareerOpportunityId, cancellationToken)
            ?? throw new KeyNotFoundException("Career opportunity not found.");

        // Idempotent: if the learner already tracks this opportunity, update the status instead of duplicating.
        var existing = await _interestRepository.GetByLearnerAndOpportunityAsync(learner.Id, opportunity.Id, cancellationToken);
        if (existing is not null)
        {
            existing.UpdateStatus(request.Status, request.Notes);
            await _interestRepository.SaveChangesAsync(cancellationToken);
            return existing.Id;
        }

        var interest = CareerInterest.Create(learner.Id, opportunity.Id, request.Status, request.Notes);
        await _interestRepository.AddAsync(interest, cancellationToken);
        await _interestRepository.SaveChangesAsync(cancellationToken);

        return interest.Id;
    }
}
