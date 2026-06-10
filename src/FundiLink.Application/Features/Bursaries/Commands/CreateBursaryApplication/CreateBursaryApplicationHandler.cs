using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Bursaries.Commands.CreateBursaryApplication;

public class CreateBursaryApplicationHandler : IRequestHandler<CreateBursaryApplicationCommand, Guid>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IBursaryRepository _bursaryRepository;
    private readonly IBursaryApplicationRepository _bursaryApplicationRepository;

    public CreateBursaryApplicationHandler(
        ILearnerRepository learnerRepository,
        IBursaryRepository bursaryRepository,
        IBursaryApplicationRepository bursaryApplicationRepository)
    {
        _learnerRepository = learnerRepository;
        _bursaryRepository = bursaryRepository;
        _bursaryApplicationRepository = bursaryApplicationRepository;
    }

    public async Task<Guid> Handle(CreateBursaryApplicationCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var bursary = await _bursaryRepository.GetByIdAsync(request.BursaryId, cancellationToken)
            ?? throw new KeyNotFoundException("Bursary not found.");

        var application = BursaryApplication.Create(
            learner.Id,
            bursary.Id,
            request.Status,
            request.Notes,
            request.DeadlineDate);

        await _bursaryApplicationRepository.AddAsync(application, cancellationToken);
        await _bursaryApplicationRepository.SaveChangesAsync(cancellationToken);

        return application.Id;
    }
}
