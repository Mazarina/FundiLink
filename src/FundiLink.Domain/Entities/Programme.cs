using System.Text.Json;
using FundiLink.Domain.Common;

namespace FundiLink.Domain.Entities;

public record RequiredSubject(string SubjectName, int MinimumPercentage);

public class Programme : BaseEntity
{
    public Guid InstitutionId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? FacultyOrSchool { get; private set; }
    public int? NfqLevel { get; private set; }
    public int MinimumAps { get; private set; }
    public string RequiredSubjectsJson { get; private set; } = "[]";
    public DateTime? ApplicationOpenDate { get; private set; }
    public DateTime? ApplicationCloseDate { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation
    public Institution Institution { get; private set; } = default!;

    /// <summary>
    /// Deserializes the stored JSON into the list of required subjects.
    /// Returns an empty list when no subjects are configured.
    /// </summary>
    public List<RequiredSubject> RequiredSubjects =>
        string.IsNullOrWhiteSpace(RequiredSubjectsJson)
            ? []
            : JsonSerializer.Deserialize<List<RequiredSubject>>(RequiredSubjectsJson) ?? [];

    private Programme() { }

    public static Programme Create(
        Guid institutionId,
        string name,
        int minimumAps,
        IEnumerable<RequiredSubject>? requiredSubjects = null,
        string? facultyOrSchool = null,
        int? nfqLevel = null,
        DateTime? applicationOpenDate = null,
        DateTime? applicationCloseDate = null,
        bool isActive = true)
    {
        return new Programme
        {
            InstitutionId = institutionId,
            Name = name,
            MinimumAps = minimumAps,
            RequiredSubjectsJson = JsonSerializer.Serialize(requiredSubjects?.ToList() ?? []),
            FacultyOrSchool = facultyOrSchool,
            NfqLevel = nfqLevel,
            ApplicationOpenDate = applicationOpenDate,
            ApplicationCloseDate = applicationCloseDate,
            IsActive = isActive
        };
    }
}
