using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.AcademicProfile.Commands.SaveAcademicProfile;

public record SaveAcademicProfileCommand(
    string UserId,
    int Year,
    ResultType ResultType,
    IReadOnlyList<SubjectInputDto> Subjects
) : IRequest<SaveAcademicProfileResult>;

public record SubjectInputDto(
    string SubjectName,
    int Percentage,
    bool IsHomeLanguage,
    bool IsLifeOrientation,
    string? SubjectCode = null
);

public record SaveAcademicProfileResult(int ApsScore, int SubjectCount);
