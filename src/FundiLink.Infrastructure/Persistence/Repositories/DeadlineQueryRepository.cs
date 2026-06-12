using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

/// <summary>
/// Deterministic, read-only query over upcoming programme and bursary application deadlines.
/// Joins each application to its opportunity name and restricts to active (non-deleted)
/// records whose deadline falls inside the requested window. Returns only the minimal fields
/// needed to compose a guidance reminder.
/// </summary>
public class DeadlineQueryRepository : IDeadlineQueryRepository
{
    private readonly FundiLinkDbContext _db;

    public DeadlineQueryRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<UpcomingDeadline>> GetUpcomingDeadlinesAsync(
        DateTime fromInclusive, DateTime toInclusive, CancellationToken ct)
    {
        var activeLearnerIds = _db.Learners
            .Where(l => l.DeletedAt == null)
            .Select(l => l.Id);

        var programmeDeadlines = await _db.LearnerApplications
            .Where(a => a.DeletedAt == null
                        && a.DeadlineDate != null
                        && a.DeadlineDate >= fromInclusive
                        && a.DeadlineDate <= toInclusive
                        && activeLearnerIds.Contains(a.LearnerId))
            .Join(_db.Programmes,
                a => a.ProgrammeId,
                p => p.Id,
                (a, p) => new UpcomingDeadline(
                    a.LearnerId,
                    DeadlineKind.ProgrammeApplication,
                    p.Name,
                    a.DeadlineDate!.Value))
            .ToListAsync(ct);

        var bursaryDeadlines = await _db.BursaryApplications
            .Where(a => a.DeletedAt == null
                        && a.DeadlineDate != null
                        && a.DeadlineDate >= fromInclusive
                        && a.DeadlineDate <= toInclusive
                        && activeLearnerIds.Contains(a.LearnerId))
            .Join(_db.Bursaries,
                a => a.BursaryId,
                b => b.Id,
                (a, b) => new UpcomingDeadline(
                    a.LearnerId,
                    DeadlineKind.BursaryApplication,
                    b.Name,
                    a.DeadlineDate!.Value))
            .ToListAsync(ct);

        return programmeDeadlines.Concat(bursaryDeadlines).ToList();
    }

    public async Task<IReadOnlyList<UpcomingDeadline>> GetUpcomingDeadlinesForLearnerAsync(
        Guid learnerId, DateTime fromInclusive, DateTime toInclusive, CancellationToken ct)
    {
        var programmeDeadlines = await _db.LearnerApplications
            .Where(a => a.DeletedAt == null
                        && a.LearnerId == learnerId
                        && a.DeadlineDate != null
                        && a.DeadlineDate >= fromInclusive
                        && a.DeadlineDate <= toInclusive)
            .Join(_db.Programmes,
                a => a.ProgrammeId,
                p => p.Id,
                (a, p) => new UpcomingDeadline(
                    a.LearnerId,
                    DeadlineKind.ProgrammeApplication,
                    p.Name,
                    a.DeadlineDate!.Value))
            .ToListAsync(ct);

        var bursaryDeadlines = await _db.BursaryApplications
            .Where(a => a.DeletedAt == null
                        && a.LearnerId == learnerId
                        && a.DeadlineDate != null
                        && a.DeadlineDate >= fromInclusive
                        && a.DeadlineDate <= toInclusive)
            .Join(_db.Bursaries,
                a => a.BursaryId,
                b => b.Id,
                (a, b) => new UpcomingDeadline(
                    a.LearnerId,
                    DeadlineKind.BursaryApplication,
                    b.Name,
                    a.DeadlineDate!.Value))
            .ToListAsync(ct);

        return programmeDeadlines
            .Concat(bursaryDeadlines)
            .OrderBy(d => d.DeadlineDate)
            .ToList();
    }
}
