using System.Text.Json;
using FundiLink.Domain.Common;
using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

/// <summary>
/// Curated early-career opportunity (learnership, internship, skills programme) for guidance only.
/// FundiLink is NOT an employer or recruitment agency and does not submit applications or guarantee placement.
/// </summary>
public class CareerOpportunity : BaseEntity
{
    public string Title { get; private set; } = default!;
    public string ProviderName { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public CareerOpportunityType OpportunityType { get; private set; }
    public string FieldsOfInterestJson { get; private set; } = "[]";

    /// <summary>Minimum grade level required. Null means "no minimum".</summary>
    public GradeLevel? MinimumGradeLevel { get; private set; }

    public string ProvincesEligibleJson { get; private set; } = "[]";
    public DateTime? ApplicationCloseDate { get; private set; }
    public string? ExternalApplicationUrl { get; private set; }
    public bool IsActive { get; private set; } = true;

    /// <summary>Fields of interest this opportunity relates to. Empty list means "any field".</summary>
    public List<string> FieldsOfInterest =>
        string.IsNullOrWhiteSpace(FieldsOfInterestJson)
            ? []
            : JsonSerializer.Deserialize<List<string>>(FieldsOfInterestJson) ?? [];

    /// <summary>Provinces eligible. Empty list means "all provinces".</summary>
    public List<string> ProvincesEligible =>
        string.IsNullOrWhiteSpace(ProvincesEligibleJson)
            ? []
            : JsonSerializer.Deserialize<List<string>>(ProvincesEligibleJson) ?? [];

    private CareerOpportunity() { }

    public static CareerOpportunity Create(
        string title,
        string providerName,
        string description,
        CareerOpportunityType opportunityType,
        IEnumerable<string>? fieldsOfInterest = null,
        GradeLevel? minimumGradeLevel = null,
        IEnumerable<string>? provincesEligible = null,
        DateTime? applicationCloseDate = null,
        string? externalApplicationUrl = null,
        bool isActive = true)
    {
        return new CareerOpportunity
        {
            Title = title,
            ProviderName = providerName,
            Description = description,
            OpportunityType = opportunityType,
            FieldsOfInterestJson = JsonSerializer.Serialize(fieldsOfInterest?.ToList() ?? []),
            MinimumGradeLevel = minimumGradeLevel,
            ProvincesEligibleJson = JsonSerializer.Serialize(provincesEligible?.ToList() ?? []),
            ApplicationCloseDate = applicationCloseDate,
            ExternalApplicationUrl = externalApplicationUrl,
            IsActive = isActive
        };
    }
}
