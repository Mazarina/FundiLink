using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.AcademicProfile.Services;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.AcademicProfile.Commands.SaveAcademicProfile;

public class SaveAcademicProfileHandler : IRequestHandler<SaveAcademicProfileCommand, SaveAcademicProfileResult>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly ApsCalculatorService _apsCalculator;

    public SaveAcademicProfileHandler(
        ILearnerRepository learnerRepository,
        IApplicationDbContext dbContext,
        ApsCalculatorService apsCalculator)
    {
        _learnerRepository = learnerRepository;
        _dbContext = dbContext;
        _apsCalculator = apsCalculator;
    }

    public async Task<SaveAcademicProfileResult> Handle(SaveAcademicProfileCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var profile = await _learnerRepository.GetAcademicProfileByLearnerIdAsync(learner.Id, cancellationToken);

        if (profile is null)
        {
            profile = Domain.Entities.AcademicProfile.Create(learner.Id, request.Year, request.ResultType);
            await _learnerRepository.AddAcademicProfileAsync(profile, cancellationToken);
        }

        var subjects = request.Subjects.Select(s =>
            NscSubjectResult.Create(profile.Id, s.SubjectName, s.Percentage, s.IsHomeLanguage, s.IsLifeOrientation, s.SubjectCode)
        ).ToList();

        profile.SetSubjects(subjects);

        var apsScore = _apsCalculator.CalculateAps(subjects);
        profile.SetApsScore(apsScore);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SaveAcademicProfileResult(apsScore, subjects.Count);
    }
}
