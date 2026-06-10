using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

/// <summary>
/// Append-only record of an AI assistant interaction.
/// POPIA-minimal: stores only the learner, the intent, and when it happened —
/// never the answer text or any sensitive profile detail. Immutable after creation.
/// </summary>
public class AssistantInteractionLog
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LearnerId { get; private set; }
    public AssistantIntent Intent { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AssistantInteractionLog() { }

    public static AssistantInteractionLog Create(Guid learnerId, AssistantIntent intent)
    {
        return new AssistantInteractionLog
        {
            Id = Guid.NewGuid(),
            LearnerId = learnerId,
            Intent = intent,
            CreatedAt = DateTime.UtcNow
        };
    }
}
