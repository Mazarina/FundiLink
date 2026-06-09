namespace FundiLink.Domain.Entities;

/// <summary>
/// Append-only audit record of sensitive admin/support actions.
/// Never updated or deleted.
/// </summary>
public class AuditLogEntry
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string ActorUserId { get; private set; } = default!;
    public string ActorRole { get; private set; } = default!;
    public string Action { get; private set; } = default!;
    public string TargetType { get; private set; } = default!;
    public string TargetId { get; private set; } = default!;
    public DateTime OccurredAt { get; private set; }

    private AuditLogEntry() { }

    public static AuditLogEntry Create(
        string actorUserId,
        string actorRole,
        string action,
        string targetType,
        string targetId)
    {
        return new AuditLogEntry
        {
            Id = Guid.NewGuid(),
            ActorUserId = actorUserId,
            ActorRole = actorRole,
            Action = action,
            TargetType = targetType,
            TargetId = targetId,
            OccurredAt = DateTime.UtcNow
        };
    }
}
