namespace FundiLink.Application.Common.Interfaces;

/// <summary>
/// Erasure fulfilment service (POPIA right to erasure). Deterministic, in-process
/// implementation: anonymises/soft-deletes a learner's personal data while preserving
/// append-only audit and consent records (POPIA proof-of-processing retention).
/// NEVER touches audit or consent logs. No external storage/email/delivery provider in
/// this phase; a real delivery channel may be wired later behind the same interface
/// (any key supplied via environment only).
/// </summary>
public interface IErasureService
{
    /// <summary>
    /// Anonymises the learner's personal data: redacts the learner profile and removes
    /// the academic profile, application/bursary tracking, document metadata and interests.
    /// Returns a summary of what was anonymised/removed for audit purposes.
    /// </summary>
    Task<ErasureOutcome> AnonymiseLearnerDataAsync(Guid learnerId, CancellationToken ct);
}

/// <summary>Summary of the data anonymised/removed during an erasure fulfilment.</summary>
public record ErasureOutcome(
    int DocumentsRemoved,
    int ApplicationsRemoved,
    int BursaryApplicationsRemoved,
    int AccommodationInterestsRemoved,
    int CareerInterestsRemoved,
    bool AcademicProfileRemoved,
    bool ProfileAnonymised
);
