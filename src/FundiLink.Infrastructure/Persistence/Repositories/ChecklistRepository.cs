using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

public class ChecklistRepository : IChecklistRepository
{
    private readonly FundiLinkDbContext _db;

    public ChecklistRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<DocumentChecklistItem>> GetByApplicationIdAsync(Guid applicationId, CancellationToken ct)
        => await _db.DocumentChecklistItems
            .Where(c => c.LearnerApplicationId == applicationId)
            .ToListAsync(ct);

    public async Task<DocumentChecklistItem?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.DocumentChecklistItems.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task AddAsync(DocumentChecklistItem item, CancellationToken ct)
        => await _db.DocumentChecklistItems.AddAsync(item, ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
