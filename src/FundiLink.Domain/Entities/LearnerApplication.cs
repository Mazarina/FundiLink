using FundiLink.Domain.Common;
using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

public class LearnerApplication : BaseEntity
{
    public Guid LearnerId { get; private set; }
    public Guid ProgrammeId { get; private set; }
    public ApplicationStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? DeadlineDate { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
    public DateTime? OutcomeAt { get; private set; }

    // Navigation
    public Programme Programme { get; private set; } = default!;

    private LearnerApplication() { }

    public static LearnerApplication Create(
        Guid learnerId,
        Guid programmeId,
        ApplicationStatus status = ApplicationStatus.Interested,
        string? notes = null,
        DateTime? deadlineDate = null)
    {
        var application = new LearnerApplication
        {
            LearnerId = learnerId,
            ProgrammeId = programmeId,
            Status = status,
            Notes = notes,
            DeadlineDate = deadlineDate
        };
        application.ApplyStatusSideEffects(status);
        return application;
    }

    public void UpdateStatus(ApplicationStatus newStatus, string? notes)
    {
        Status = newStatus;
        if (notes is not null) Notes = notes;
        ApplyStatusSideEffects(newStatus);
        MarkUpdated();
    }

    private void ApplyStatusSideEffects(ApplicationStatus status)
    {
        if (status == ApplicationStatus.Submitted && SubmittedAt is null)
            SubmittedAt = DateTime.UtcNow;

        if (status is ApplicationStatus.Accepted or ApplicationStatus.Rejected or ApplicationStatus.Waitlisted)
            OutcomeAt = DateTime.UtcNow;
    }
}
