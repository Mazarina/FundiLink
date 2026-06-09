using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Applications.Commands.CreateApplication;

public class CreateApplicationHandler : IRequestHandler<CreateApplicationCommand, Guid>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IProgrammeRepository _programmeRepository;
    private readonly IApplicationRepository _applicationRepository;

    public CreateApplicationHandler(
        ILearnerRepository learnerRepository,
        IProgrammeRepository programmeRepository,
        IApplicationRepository applicationRepository)
    {
        _learnerRepository = learnerRepository;
        _programmeRepository = programmeRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<Guid> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var programme = await _programmeRepository.GetByIdAsync(request.ProgrammeId, cancellationToken)
            ?? throw new KeyNotFoundException("Programme not found.");

        var application = LearnerApplication.Create(
            learner.Id,
            programme.Id,
            request.Status,
            request.Notes,
            request.DeadlineDate);

        await _applicationRepository.AddAsync(application, cancellationToken);
        await _applicationRepository.SaveChangesAsync(cancellationToken);

        return application.Id;
    }
}
