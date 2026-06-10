using FundiLink.Domain.Common;
using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

public class BursaryApplication : BaseEntity
{
    public Guid LearnerId { get; private set; }
    public Guid BursaryId { get; private set; }
    public BursaryApplicationStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? DeadlineDate { get; private set; }

    // Navigation
    public Bursary Bursary { get; private set; } = default!;

    private BursaryApplication() { }

    public static BursaryApplication Create(
        Guid learnerId,
        Guid bursaryId,
        BursaryApplicationStatus status = BursaryApplicationStatus.Researching,
        string? notes = null,
        DateTime? deadlineDate = null)
    {
        return new BursaryApplication
        {
            LearnerId = learnerId,
            BursaryId = bursaryId,
            Status = status,
            Notes = notes,
            DeadlineDate = deadlineDate
        };
    }

    public void UpdateStatus(BursaryApplicationStatus newStatus, string? notes)
    {
        Status = newStatus;
        if (notes is not null) Notes = notes;
        MarkUpdated();
    }
}
