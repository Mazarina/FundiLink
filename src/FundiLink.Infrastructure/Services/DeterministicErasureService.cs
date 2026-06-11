using FundiLink.Application.Common.Interfaces;
using FundiLink.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Infrastructure.Services;

/// <summary>
/// Deterministic, in-process erasure fulfilment (POPIA right to erasure). Anonymises the
/// learner profile and removes the learner's personal data — academic profile and subject
/// results, application and bursary-application tracking, document metadata, and accommodation/
/// career interests. It NEVER touches append-only audit (<c>AuditLogEntries</c>) or consent
/// (<c>GuardianConsents</c>) records, which are POPIA-minimal and retained as proof of lawful
/// processing/consent. No external storage/email/delivery provider is called in this phase; a
/// real delivery channel may be wired later behind <see cref="IErasureService"/> (key via env).
/// </summary>
public class DeterministicErasureService : IErasureService
{
    private readonly FundiLinkDbContext _db;

    public DeterministicErasureService(FundiLinkDbContext db)
    {
        _db = db;
    }

    public async Task<ErasureOutcome> AnonymiseLearnerDataAsync(Guid learnerId, CancellationToken ct)
    {
        // IgnoreQueryFilters throughout — erasure must reach all of the learner's personal
        // data, including any rows already soft-deleted, leaving nothing behind.
        var learner = await _db.Learners.IgnoreQueryFilters()
            .FirstOrDefaultAsync(l => l.Id == learnerId, ct)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        // Academic profile + subject results.
        var academic = await _db.AcademicProfiles.IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.LearnerId == learnerId, ct);
        var academicRemoved = false;
        if (academic is not null)
        {
            var subjects = await _db.NscSubjectResults.IgnoreQueryFilters()
                .Where(s => s.AcademicProfileId == academic.Id)
                .ToListAsync(ct);
            _db.NscSubjectResults.RemoveRange(subjects);
            _db.AcademicProfiles.Remove(academic);
            academicRemoved = true;
        }

        var applications = await _db.LearnerApplications.IgnoreQueryFilters()
            .Where(a => a.LearnerId == learnerId).ToListAsync(ct);
        _db.LearnerApplications.RemoveRange(applications);

        var bursaryApplications = await _db.BursaryApplications.IgnoreQueryFilters()
            .Where(a => a.LearnerId == learnerId).ToListAsync(ct);
        _db.BursaryApplications.RemoveRange(bursaryApplications);

        var documents = await _db.Documents.IgnoreQueryFilters()
            .Where(d => d.LearnerId == learnerId).ToListAsync(ct);
        _db.Documents.RemoveRange(documents);

        var accommodationInterests = await _db.AccommodationInterests.IgnoreQueryFilters()
            .Where(i => i.LearnerId == learnerId).ToListAsync(ct);
        _db.AccommodationInterests.RemoveRange(accommodationInterests);

        var careerInterests = await _db.CareerInterests.IgnoreQueryFilters()
            .Where(i => i.LearnerId == learnerId).ToListAsync(ct);
        _db.CareerInterests.RemoveRange(careerInterests);

        // Append-only audit (AuditLogEntries) and consent (GuardianConsents) records are
        // deliberately left untouched — POPIA proof-of-processing retention.
        learner.Anonymise();

        await _db.SaveChangesAsync(ct);

        return new ErasureOutcome(
            DocumentsRemoved: documents.Count,
            ApplicationsRemoved: applications.Count,
            BursaryApplicationsRemoved: bursaryApplications.Count,
            AccommodationInterestsRemoved: accommodationInterests.Count,
            CareerInterestsRemoved: careerInterests.Count,
            AcademicProfileRemoved: academicRemoved,
            ProfileAnonymised: true);
    }
}
