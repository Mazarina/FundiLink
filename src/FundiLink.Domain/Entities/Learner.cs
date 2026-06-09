using FundiLink.Domain.Common;
using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

public class Learner : BaseEntity
{
    public string UserId { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string Surname { get; private set; } = default!;
    public DateOnly DateOfBirth { get; private set; }
    public string? IdNumber { get; private set; }
    public string? PassportNumber { get; private set; }
    public string? Gender { get; private set; }
    public string? HomeLanguage { get; private set; }
    public string Nationality { get; private set; } = "South African";
    public string MobileNumber { get; private set; } = default!;
    public string Province { get; private set; } = default!;
    public string Municipality { get; private set; } = string.Empty;
    public string Suburb { get; private set; } = string.Empty;
    public string SchoolName { get; private set; } = default!;
    public string SchoolProvince { get; private set; } = default!;
    public GradeLevel GradeLevel { get; private set; }

    // Guardian info — required for learners under 18
    public string? GuardianName { get; private set; }
    public string? GuardianPhone { get; private set; }
    public string? GuardianEmail { get; private set; }

    // POPIA consent
    public bool ConsentAccepted { get; private set; }
    public DateTime ConsentTimestamp { get; private set; }
    public string ConsentVersion { get; private set; } = default!;
    public bool? GuardianConsentAccepted { get; private set; }
    public DateTime? GuardianConsentTimestamp { get; private set; }

    public int ProfileCompleteness { get; private set; }

    // Navigation
    public AcademicProfile? AcademicProfile { get; private set; }

    private Learner() { }

    public static Learner Create(
        string userId,
        string firstName,
        string surname,
        DateOnly dateOfBirth,
        string mobileNumber,
        string province,
        string schoolName,
        string schoolProvince,
        GradeLevel gradeLevel,
        bool consentAccepted,
        string consentVersion)
    {
        if (!consentAccepted)
            throw new InvalidOperationException("POPIA consent is required to create a learner profile.");

        return new Learner
        {
            UserId = userId,
            FirstName = firstName,
            Surname = surname,
            DateOfBirth = dateOfBirth,
            MobileNumber = mobileNumber,
            Province = province,
            SchoolName = schoolName,
            SchoolProvince = schoolProvince,
            GradeLevel = gradeLevel,
            ConsentAccepted = consentAccepted,
            ConsentTimestamp = DateTime.UtcNow,
            ConsentVersion = consentVersion,
            ProfileCompleteness = 20
        };
    }

    public bool IsMinor() => DateOfBirth.ToDateTime(TimeOnly.MinValue) > DateTime.UtcNow.AddYears(-18);

    public void UpdatePersonalInfo(
        string firstName,
        string surname,
        string? idNumber,
        string? passportNumber,
        string? gender,
        string? homeLanguage,
        string nationality,
        string mobileNumber,
        string province,
        string municipality,
        string suburb,
        string schoolName,
        string schoolProvince,
        GradeLevel gradeLevel,
        string? guardianName,
        string? guardianPhone,
        string? guardianEmail)
    {
        FirstName = firstName;
        Surname = surname;
        IdNumber = idNumber;
        PassportNumber = passportNumber;
        Gender = gender;
        HomeLanguage = homeLanguage;
        Nationality = nationality;
        MobileNumber = mobileNumber;
        Province = province;
        Municipality = municipality;
        Suburb = suburb;
        SchoolName = schoolName;
        SchoolProvince = schoolProvince;
        GradeLevel = gradeLevel;
        GuardianName = guardianName;
        GuardianPhone = guardianPhone;
        GuardianEmail = guardianEmail;

        RecalculateCompleteness();
        MarkUpdated();
    }

    public void RecalculateCompleteness()
    {
        int score = 20; // base for having an account
        if (!string.IsNullOrEmpty(IdNumber) || !string.IsNullOrEmpty(PassportNumber)) score += 10;
        if (!string.IsNullOrEmpty(MobileNumber)) score += 10;
        if (!string.IsNullOrEmpty(Province)) score += 10;
        if (!string.IsNullOrEmpty(SchoolName)) score += 10;
        if (AcademicProfile != null && AcademicProfile.Subjects.Count > 0) score += 30;
        if (!string.IsNullOrEmpty(GuardianName) || !IsMinor()) score += 10;
        ProfileCompleteness = Math.Min(score, 100);
    }
}
