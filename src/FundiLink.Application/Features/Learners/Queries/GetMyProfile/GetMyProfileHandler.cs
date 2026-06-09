using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Learners.Queries.GetMyProfile;

public class GetMyProfileHandler : IRequestHandler<GetMyProfileQuery, LearnerProfileDto>
{
    private readonly ILearnerRepository _learnerRepository;

    public GetMyProfileHandler(ILearnerRepository learnerRepository)
    {
        _learnerRepository = learnerRepository;
    }

    public async Task<LearnerProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        // Mask the ID number — never return in full
        var maskedId = learner.IdNumber is not null
            ? "***" + learner.IdNumber[^4..]
            : null;

        return new LearnerProfileDto(
            learner.Id,
            learner.FirstName,
            learner.Surname,
            learner.DateOfBirth,
            maskedId,
            learner.Gender,
            learner.HomeLanguage,
            learner.Nationality,
            learner.MobileNumber,
            learner.Province,
            learner.Municipality,
            learner.Suburb,
            learner.SchoolName,
            learner.SchoolProvince,
            learner.GradeLevel,
            learner.GuardianName,
            learner.GuardianPhone,
            learner.GuardianEmail,
            learner.IsMinor(),
            learner.ProfileCompleteness
        );
    }
}
