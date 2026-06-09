using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.AcademicProfile.Queries.GetAcademicProfile;

public class GetAcademicProfileHandler : IRequestHandler<GetAcademicProfileQuery, AcademicProfileDto?>
{
    private readonly ILearnerRepository _learnerRepository;

    public GetAcademicProfileHandler(ILearnerRepository learnerRepository)
    {
        _learnerRepository = learnerRepository;
    }

    public async Task<AcademicProfileDto?> Handle(GetAcademicProfileQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var profile = await _learnerRepository.GetAcademicProfileByLearnerIdAsync(learner.Id, cancellationToken);
        if (profile is null) return null;

        return new AcademicProfileDto(
            profile.Id,
            profile.Year,
            profile.ResultType,
            profile.ApsScore,
            profile.ApsCalculatedAt,
            profile.Subjects.Select(s => new SubjectResultDto(
                s.SubjectName, s.SubjectCode, s.Percentage, s.ApsPoints,
                s.IsHomeLanguage, s.IsLifeOrientation)).ToList()
        );
    }
}
