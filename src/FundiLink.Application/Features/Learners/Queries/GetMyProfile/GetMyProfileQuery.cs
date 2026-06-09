using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Learners.Queries.GetMyProfile;

public record GetMyProfileQuery(string UserId) : IRequest<LearnerProfileDto>;

public record LearnerProfileDto(
    Guid Id,
    string FirstName,
    string Surname,
    DateOnly DateOfBirth,
    string? IdNumberMasked,
    string? Gender,
    string? HomeLanguage,
    string Nationality,
    string MobileNumber,
    string Province,
    string Municipality,
    string Suburb,
    string SchoolName,
    string SchoolProvince,
    GradeLevel GradeLevel,
    string? GuardianName,
    string? GuardianPhone,
    string? GuardianEmail,
    bool IsMinor,
    int ProfileCompleteness
);
