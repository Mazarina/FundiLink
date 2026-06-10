using System.Text.Json;
using FundiLink.Domain.Common;
using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

public class Bursary : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string ProviderName { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public BursaryFundingType FundingType { get; private set; }
    public string FieldsOfStudyJson { get; private set; } = "[]";
    public int? MinimumAps { get; private set; }
    public decimal? MaxHouseholdIncome { get; private set; }
    public string ProvincesEligibleJson { get; private set; } = "[]";
    public DateTime? ApplicationOpenDate { get; private set; }
    public DateTime? ApplicationCloseDate { get; private set; }
    public string? ExternalApplicationUrl { get; private set; }
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Fields of study this bursary supports. Empty list means "any field".
    /// </summary>
    public List<string> FieldsOfStudy =>
        string.IsNullOrWhiteSpace(FieldsOfStudyJson)
            ? []
            : JsonSerializer.Deserialize<List<string>>(FieldsOfStudyJson) ?? [];

    /// <summary>
    /// Provinces eligible for this bursary. Empty list means "all provinces".
    /// </summary>
    public List<string> ProvincesEligible =>
        string.IsNullOrWhiteSpace(ProvincesEligibleJson)
            ? []
            : JsonSerializer.Deserialize<List<string>>(ProvincesEligibleJson) ?? [];

    private Bursary() { }

    public static Bursary Create(
        string name,
        string providerName,
        string description,
        BursaryFundingType fundingType,
        IEnumerable<string>? fieldsOfStudy = null,
        int? minimumAps = null,
        decimal? maxHouseholdIncome = null,
        IEnumerable<string>? provincesEligible = null,
        DateTime? applicationOpenDate = null,
        DateTime? applicationCloseDate = null,
        string? externalApplicationUrl = null,
        bool isActive = true)
    {
        return new Bursary
        {
            Name = name,
            ProviderName = providerName,
            Description = description,
            FundingType = fundingType,
            FieldsOfStudyJson = JsonSerializer.Serialize(fieldsOfStudy?.ToList() ?? []),
            MinimumAps = minimumAps,
            MaxHouseholdIncome = maxHouseholdIncome,
            ProvincesEligibleJson = JsonSerializer.Serialize(provincesEligible?.ToList() ?? []),
            ApplicationOpenDate = applicationOpenDate,
            ApplicationCloseDate = applicationCloseDate,
            ExternalApplicationUrl = externalApplicationUrl,
            IsActive = isActive
        };
    }

    public void Update(
        string name,
        string providerName,
        string description,
        BursaryFundingType fundingType,
        IEnumerable<string>? fieldsOfStudy,
        int? minimumAps,
        decimal? maxHouseholdIncome,
        IEnumerable<string>? provincesEligible,
        DateTime? applicationOpenDate,
        DateTime? applicationCloseDate,
        string? externalApplicationUrl,
        bool isActive)
    {
        Name = name;
        ProviderName = providerName;
        Description = description;
        FundingType = fundingType;
        FieldsOfStudyJson = JsonSerializer.Serialize(fieldsOfStudy?.ToList() ?? []);
        MinimumAps = minimumAps;
        MaxHouseholdIncome = maxHouseholdIncome;
        ProvincesEligibleJson = JsonSerializer.Serialize(provincesEligible?.ToList() ?? []);
        ApplicationOpenDate = applicationOpenDate;
        ApplicationCloseDate = applicationCloseDate;
        ExternalApplicationUrl = externalApplicationUrl;
        IsActive = isActive;
        MarkUpdated();
    }
}
