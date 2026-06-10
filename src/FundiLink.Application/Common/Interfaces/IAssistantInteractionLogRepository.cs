using FundiLink.Domain.Entities;

namespace FundiLink.Application.Common.Interfaces;

public interface IAssistantInteractionLogRepository
{
    // Append-only — no update or delete operations are exposed.
    Task AddAsync(AssistantInteractionLog log, CancellationToken ct);
    Task<IEnumerable<AssistantInteractionLog>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
