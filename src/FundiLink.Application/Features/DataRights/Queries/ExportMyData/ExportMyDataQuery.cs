using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.DataRights.Queries.ExportMyData;

/// <summary>
/// Requests a structured export of the caller's own FundiLink data (POPIA right of access).
/// Owner-scoped: the learner is resolved from the authenticated user id.
/// </summary>
public record ExportMyDataQuery(string UserId) : IRequest<DataExportDto>;

/// <summary>Typed, owner-scoped export of a learner's FundiLink data.</summary>
public record DataExportDto(
    DateTime GeneratedAt,
    ExportProfileDto Profile,
    ExportAcademicProfileDto? AcademicProfile,
    IReadOnlyList<ExportApplicationDto> Applications,
    IReadOnlyList<ExportBursaryApplicationDto> BursaryApplications,
    IReadOnlyList<ExportDocumentDto> Documents,
    IReadOnlyList<ExportInterestDto> AccommodationInterests,
    IReadOnlyList<ExportInterestDto> CareerInterests,
    IReadOnlyList<ExportConsentDto> ConsentHistory,
    string Disclaimer
);

public record ExportProfileDto(
    Guid LearnerId,
    string FirstName,
    string Surname,
    DateOnly DateOfBirth,
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
    string? GuardianEmail,
    int ProfileCompleteness,
    DateTime CreatedAt
);

public record ExportAcademicProfileDto(
    int Year,
    ResultType ResultType,
    int ApsScore,
    DateTime? ApsCalculatedAt,
    IReadOnlyList<ExportSubjectDto> Subjects
);

public record ExportSubjectDto(string SubjectName, int Percentage, int Level);

public record ExportApplicationDto(
    Guid Id,
    string ProgrammeName,
    ApplicationStatus Status,
    DateTime? DeadlineDate,
    DateTime? SubmittedAt,
    DateTime? OutcomeAt
);

public record ExportBursaryApplicationDto(
    Guid Id,
    string BursaryName,
    BursaryApplicationStatus Status,
    DateTime? DeadlineDate
);

public record ExportDocumentDto(
    Guid Id,
    DocumentType DocumentType,
    string FileName,
    DocumentStatus Status,
    DateTime CreatedAt
);

public record ExportInterestDto(
    Guid Id,
    string OpportunityName,
    OpportunityInterestStatus Status
);

public record ExportConsentDto(
    ConsentType ConsentType,
    ConsentScope Scope,
    ConsentStatus Status,
    DateTime RecordedAt
);
