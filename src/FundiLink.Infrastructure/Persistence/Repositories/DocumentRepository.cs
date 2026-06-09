using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly FundiLinkDbContext _db;

    public DocumentRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<Document?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.Documents.FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<IEnumerable<Document>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct)
        => await _db.Documents
            .Where(d => d.LearnerId == learnerId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Document document, CancellationToken ct)
        => await _db.Documents.AddAsync(document, ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}
