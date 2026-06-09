using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.AcademicProfile.Queries.GetAcademicProfile;

public record GetAcademicProfileQuery(string UserId) : IRequest<AcademicProfileDto?>;

public record AcademicProfileDto(
    Guid Id,
    int Year,
    ResultType ResultType,
    int ApsScore,
    DateTime? ApsCalculatedAt,
    IReadOnlyList<SubjectResultDto> Subjects
);

public record SubjectResultDto(
    string SubjectName,
    string? SubjectCode,
    int Percentage,
    int ApsPoints,
    bool IsHomeLanguage,
    bool IsLifeOrientation
);
