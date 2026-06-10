using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Accommodation.Commands.TrackAccommodationInterest;

public class TrackAccommodationInterestHandler : IRequestHandler<TrackAccommodationInterestCommand, Guid>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IAccommodationRepository _accommodationRepository;
    private readonly IAccommodationInterestRepository _interestRepository;

    public TrackAccommodationInterestHandler(
        ILearnerRepository learnerRepository,
        IAccommodationRepository accommodationRepository,
        IAccommodationInterestRepository interestRepository)
    {
        _learnerRepository = learnerRepository;
        _accommodationRepository = accommodationRepository;
        _interestRepository = interestRepository;
    }

    public async Task<Guid> Handle(TrackAccommodationInterestCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var listing = await _accommodationRepository.GetByIdAsync(request.AccommodationListingId, cancellationToken)
            ?? throw new KeyNotFoundException("Accommodation listing not found.");

        // Idempotent: if the learner already tracks this listing, update the status instead of duplicating.
        var existing = await _interestRepository.GetByLearnerAndListingAsync(learner.Id, listing.Id, cancellationToken);
        if (existing is not null)
        {
            existing.UpdateStatus(request.Status, request.Notes);
            await _interestRepository.SaveChangesAsync(cancellationToken);
            return existing.Id;
        }

        var interest = AccommodationInterest.Create(learner.Id, listing.Id, request.Status, request.Notes);
        await _interestRepository.AddAsync(interest, cancellationToken);
        await _interestRepository.SaveChangesAsync(cancellationToken);

        return interest.Id;
    }
}
