using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

/// <summary>
/// Append-only assistant interaction log repository. No update or delete operations are exposed.
/// </summary>
public class AssistantInteractionLogRepository : IAssistantInteractionLogRepository
{
    private readonly FundiLinkDbContext _db;

    public AssistantInteractionLogRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(AssistantInteractionLog log, CancellationToken ct)
        => await _db.AssistantInteractionLogs.AddAsync(log, ct);

    public async Task<IEnumerable<AssistantInteractionLog>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct)
        => await _db.AssistantInteractionLogs
            .Where(l => l.LearnerId == learnerId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
