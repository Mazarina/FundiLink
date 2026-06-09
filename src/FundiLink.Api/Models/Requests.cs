using FundiLink.Domain.Enums;

namespace FundiLink.Api.Models;

public record UpdatePersonalInfoRequest(
    string FirstName,
    string Surname,
    string? IdNumber,
    string? PassportNumber,
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
    string? GuardianEmail
);

public record SaveAcademicProfileRequest(
    int Year,
    ResultType ResultType,
    IReadOnlyList<SubjectInputRequest> Subjects
);

public record SubjectInputRequest(
    string SubjectName,
    int Percentage,
    bool IsHomeLanguage,
    bool IsLifeOrientation,
    string? SubjectCode = null
);

public record CreateApplicationRequest(Guid ProgrammeId, ApplicationStatus Status, string? Notes, DateTime? DeadlineDate);

public record UpdateApplicationStatusRequest(ApplicationStatus NewStatus, string? Notes);
