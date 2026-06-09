using FundiLink.Domain.Common;

namespace FundiLink.Domain.Entities;

public class NscSubjectResult : BaseEntity
{
    public Guid AcademicProfileId { get; private set; }
    public string SubjectName { get; private set; } = default!;
    public string? SubjectCode { get; private set; }
    public int Percentage { get; private set; }
    public int ApsPoints { get; private set; }
    public bool IsHomeLanguage { get; private set; }
    public bool IsLifeOrientation { get; private set; }

    // Navigation
    public AcademicProfile AcademicProfile { get; private set; } = default!;

    private NscSubjectResult() { }

    public static NscSubjectResult Create(
        Guid academicProfileId,
        string subjectName,
        int percentage,
        bool isHomeLanguage = false,
        bool isLifeOrientation = false,
        string? subjectCode = null)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentOutOfRangeException(nameof(percentage), "Percentage must be between 0 and 100.");

        return new NscSubjectResult
        {
            AcademicProfileId = academicProfileId,
            SubjectName = subjectName,
            SubjectCode = subjectCode,
            Percentage = percentage,
            ApsPoints = CalculateApsPoints(percentage),
            IsHomeLanguage = isHomeLanguage,
            IsLifeOrientation = isLifeOrientation
        };
    }

    private static int CalculateApsPoints(int percentage) => percentage switch
    {
        >= 80 => 7,
        >= 70 => 6,
        >= 60 => 5,
        >= 50 => 4,
        >= 40 => 3,
        >= 30 => 2,
        _ => 1
    };
}
