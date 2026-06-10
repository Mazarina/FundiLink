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

public record RejectDocumentRequest(string Reason);
public record LinkDocumentRequest(Guid ChecklistItemId, Guid DocumentId);
public record CreateInstitutionRequest(string Name, InstitutionType InstitutionType, string Province, string? Website);
public record UpdateInstitutionRequest(string Name, InstitutionType InstitutionType, string Province, string? Website, bool IsActive);
public record CreateProgrammeRequest(Guid InstitutionId, string Name, string? FacultyOrSchool, int? NfqLevel, int MinimumAps, List<RequiredSubjectRequest> RequiredSubjects, DateTime? ApplicationOpenDate, DateTime? ApplicationCloseDate);
public record UpdateProgrammeRequest(string Name, string? FacultyOrSchool, int? NfqLevel, int MinimumAps, List<RequiredSubjectRequest> RequiredSubjects, DateTime? ApplicationOpenDate, DateTime? ApplicationCloseDate, bool IsActive);
public record RequiredSubjectRequest(string SubjectName, int MinimumPercentage);

public record UpdateNotificationPreferencesRequest(bool EmailEnabled, bool WhatsAppEnabled, bool SmsEnabled);

public record CreateBursaryApplicationRequest(Guid BursaryId, BursaryApplicationStatus Status, string? Notes, DateTime? DeadlineDate);
public record UpdateBursaryApplicationStatusRequest(BursaryApplicationStatus NewStatus, string? Notes);
public record CreateBursaryRequest(
    string Name,
    string ProviderName,
    string Description,
    BursaryFundingType FundingType,
    List<string> FieldsOfStudy,
    int? MinimumAps,
    decimal? MaxHouseholdIncome,
    List<string> ProvincesEligible,
    DateTime? ApplicationOpenDate,
    DateTime? ApplicationCloseDate,
    string? ExternalApplicationUrl);
public record UpdateBursaryRequest(
    string Name,
    string ProviderName,
    string Description,
    BursaryFundingType FundingType,
    List<string> FieldsOfStudy,
    int? MinimumAps,
    decimal? MaxHouseholdIncome,
    List<string> ProvincesEligible,
    DateTime? ApplicationOpenDate,
    DateTime? ApplicationCloseDate,
    string? ExternalApplicationUrl,
    bool IsActive);
