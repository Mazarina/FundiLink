using FundiLink.Domain.Common;
using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

/// <summary>
/// Owner-scoped record that a learner is tracking interest in a career opportunity.
/// POPIA-minimal: links a learner to an opportunity plus a status — no sensitive detail.
/// </summary>
public class CareerInterest : BaseEntity
{
    public Guid LearnerId { get; private set; }
    public Guid CareerOpportunityId { get; private set; }
    public OpportunityInterestStatus Status { get; private set; }
    public string? Notes { get; private set; }

    // Navigation
    public CareerOpportunity CareerOpportunity { get; private set; } = default!;

    private CareerInterest() { }

    public static CareerInterest Create(
        Guid learnerId,
        Guid careerOpportunityId,
        OpportunityInterestStatus status = OpportunityInterestStatus.Saved,
        string? notes = null)
    {
        return new CareerInterest
        {
            LearnerId = learnerId,
            CareerOpportunityId = careerOpportunityId,
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
