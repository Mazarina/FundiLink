using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

public class LearnerQueryRepository : ILearnerQueryRepository
{
    private readonly FundiLinkDbContext _db;

    public LearnerQueryRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<(IEnumerable<LearnerSummary> Items, int Total)> SearchAsync(
        string? keyword, string? province, int page, int pageSize, CancellationToken ct)
    {
        var query = _db.Learners.Where(l => l.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = keyword.Trim().ToLower();
            query = query.Where(l =>
                l.FirstName.ToLower().Contains(k) ||
                l.Surname.ToLower().Contains(k));
        }

        if (!string.IsNullOrWhiteSpace(province))
            query = query.Where(l => l.Province == province);

        var total = await query.CountAsync(ct);

        var joined = await query
            .OrderBy(l => l.Surname).ThenBy(l => l.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Join(_db.Users,
                l => l.UserId,
                u => u.Id,
                (l, u) => new LearnerSummary(
                    l.Id,
                    l.FirstName + " " + l.Surname,
                    u.Email ?? string.Empty,
                    l.Province,
                    l.GradeLevel.ToString(),
                    l.ProfileCompleteness))
            .ToListAsync(ct);

        return (joined, total);
    }

    public async Task<LearnerOverview?> GetOverviewAsync(Guid learnerId, CancellationToken ct)
    {
        var summary = await _db.Learners
            .Where(l => l.Id == learnerId && l.DeletedAt == null)
            .Join(_db.Users,
                l => l.UserId,
                u => u.Id,
                (l, u) => new LearnerSummary(
                    l.Id,
                    l.FirstName + " " + l.Surname,
                    u.Email ?? string.Empty,
                    l.Province,
                    l.GradeLevel.ToString(),
                    l.ProfileCompleteness))
            .FirstOrDefaultAsync(ct);

        if (summary is null) return null;

        var apsScore = await _db.AcademicProfiles
            .Where(a => a.LearnerId == learnerId)
            .OrderByDescending(a => a.ApsCalculatedAt)
            .Select(a => a.ApsScore)
            .FirstOrDefaultAsync(ct);

        var applicationCount = await _db.LearnerApplications
            .CountAsync(a => a.LearnerId == learnerId && a.DeletedAt == null, ct);

        var documentCount = await _db.Documents
            .CountAsync(d => d.LearnerId == learnerId, ct);

        return new LearnerOverview(summary, apsScore, applicationCount, documentCount);
    }
}
