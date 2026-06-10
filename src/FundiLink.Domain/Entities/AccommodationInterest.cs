using FundiLink.Domain.Common;
using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

/// <summary>
/// Owner-scoped record that a learner is tracking interest in an accommodation listing.
/// POPIA-minimal: links a learner to a listing plus a status — no sensitive detail.
/// </summary>
public class AccommodationInterest : BaseEntity
{
    public Guid LearnerId { get; private set; }
    public Guid AccommodationListingId { get; private set; }
    public OpportunityInterestStatus Status { get; private set; }
    public string? Notes { get; private set; }

    // Navigation
    public AccommodationListing AccommodationListing { get; private set; } = default!;

    private AccommodationInterest() { }

    public static AccommodationInterest Create(
        Guid learnerId,
        Guid accommodationListingId,
        OpportunityInterestStatus status = OpportunityInterestStatus.Saved,
        string? notes = null)
    {
        return new AccommodationInterest
        {
            LearnerId = learnerId,
            AccommodationListingId = accommodationListingId,
            Status = status,
            Notes = notes
        };
    }

    public void UpdateStatus(OpportunityInterestStatus newStatus, string? notes)
    {
        Status = newStatus;
        if (notes is not null) Notes = notes;
        MarkUpdated();
    }
}
