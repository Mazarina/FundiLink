using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Persistence.Repositories;

public class LearnerRepository : ILearnerRepository
{
    private readonly FundiLinkDbContext _db;

    public LearnerRepository(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<Learner?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        => await _db.Learners
            .Include(l => l.AcademicProfile)
            .FirstOrDefaultAsync(l => l.UserId == userId && l.DeletedAt == null, cancellationToken);

    public async Task<Learner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _db.Learners
            .Include(l => l.AcademicProfile)
            .FirstOrDefaultAsync(l => l.Id == id && l.DeletedAt == null, cancellationToken);

    public async Task AddAsync(Learner learner, CancellationToken cancellationToken = default)
        => await _db.Learners.AddAsync(learner, cancellationToken);

    public async Task<AcademicProfile?> GetAcademicProfileByLearnerIdAsync(Guid learnerId, CancellationToken cancellationToken = default)
        => await _db.AcademicProfiles
            .Include(a => a.Subjects)
            .FirstOrDefaultAsync(a => a.LearnerId == learnerId, cancellationToken);

    public async Task AddAcademicProfileAsync(AcademicProfile profile, CancellationToken cancellationToken = default)
        => await _db.AcademicProfiles.AddAsync(profile, cancellationToken);
}
