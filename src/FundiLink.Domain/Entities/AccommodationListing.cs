using FundiLink.Domain.Common;
using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

/// <summary>
/// Curated student accommodation listing for guidance only.
/// FundiLink is NOT a landlord, accommodation provider, or booking agent.
/// No availability, price, or safety is guaranteed. No bookings or payments occur here.
/// </summary>
public class AccommodationListing : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string ProviderName { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public AccommodationType AccommodationType { get; private set; }
    public string Province { get; private set; } = default!;
    public string City { get; private set; } = default!;

    /// <summary>Name of the institution this listing is near (free text), if any.</summary>
    public string? NearInstitution { get; private set; }

    /// <summary>Indicative monthly cost in ZAR. Guidance only — verify with the provider.</summary>
    public decimal? IndicativeMonthlyCost { get; private set; }

    public string? ContactUrl { get; private set; }
    public bool IsActive { get; private set; } = true;

    private AccommodationListing() { }

    public static AccommodationListing Create(
        string name,
        string providerName,
        string description,
        AccommodationType accommodationType,
        string province,
        string city,
        string? nearInstitution = null,
        decimal? indicativeMonthlyCost = null,
        string? contactUrl = null,
        bool isActive = true)
    {
        return new AccommodationListing
        {
            Name = name,
            ProviderName = providerName,
            Description = description,
            AccommodationType = accommodationType,
            Province = province,
            City = city,
            NearInstitution = nearInstitution,
            IndicativeMonthlyCost = indicativeMonthlyCost,
            ContactUrl = contactUrl,
            IsActive = isActive
        };
    }
}
