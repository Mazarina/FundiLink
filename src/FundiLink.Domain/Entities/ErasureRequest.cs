using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

/// <summary>
/// A learner-initiated erasure request (POPIA right to erasure).
/// The learner raises the request; an admin reviews and either rejects it or approves
/// and fulfils it. Fulfilment anonymises/soft-deletes the learner's personal data while
/// preserving append-only audit and consent records (POPIA proof-of-processing retention).
/// Status transitions are deliberate, audited admin actions — the entity itself records
/// who acted and when, and the full history is captured in the append-only audit log.
/// </summary>
public class ErasureRequest
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LearnerId { get; private set; }

    // Recorded requester identity (the owning learner's user id) at request time.
    public string RequestedByUserId { get; private set; } = default!;

    public ErasureRequestStatus Status { get; private set; }

    // Optional learner-supplied reason for the request (POPIA-minimal, free text).
    public string? Reason { get; private set; }

    public DateTime RequestedAt { get; private set; }

    // Recorded reviewer/fulfiller identity at the time of the action (POPIA-minimal).
    public string? ReviewedByUserId { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? ReviewNote { get; private set; }
    public DateTime? FulfilledAt { get; private set; }

    private ErasureRequest() { }

    /// <summary>Raises a new erasure request for a learner's own profile.</summary>
    public static ErasureRequest Raise(Guid learnerId, string requestedByUserId, string? reason)
    {
        if (learnerId == Guid.Empty)
            throw new ArgumentException("A learner is required to raise an erasure request.", nameof(learnerId));
        if (string.IsNullOrWhiteSpace(requestedByUserId))
            throw new ArgumentException("A requester is required to raise an erasure request.", nameof(requestedByUserId));

        return new ErasureRequest
        {
            Id = Guid.NewGuid(),
            LearnerId = learnerId,
            RequestedByUserId = requestedByUserId,
            Status = ErasureRequestStatus.Requested,
            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(),
            RequestedAt = DateTime.UtcNow
        };
    }

    /// <summary>Admin approves a pending request (without yet fulfilling it).</summary>
    public void Approve(string reviewedByUserId, string? note)
    {
        EnsurePending();
        Status = ErasureRequestStatus.Approved;
        StampReview(reviewedByUserId, note);
    }

    /// <summary>Admin rejects a pending request, recording a reason.</summary>
    public void Reject(string reviewedByUserId, string? note)
    {
        EnsurePending();
        Status = ErasureRequestStatus.Rejected;
        StampReview(reviewedByUserId, note);
    }

    /// <summary>
    /// Marks the request fulfilled after the learner's personal data has been anonymised.
    /// Only a Requested or Approved request may be fulfilled — a deliberate admin action.
    /// </summary>
    public void MarkFulfilled(string fulfilledByUserId, string? note)
    {
        if (Status is not (ErasureRequestStatus.Requested or ErasureRequestStatus.Approved))
            throw new InvalidOperationException("Only a requested or approved erasure request can be fulfilled.");

        Status = ErasureRequestStatus.Fulfilled;
        StampReview(fulfilledByUserId, note);
        FulfilledAt = DateTime.UtcNow;
    }

    private void EnsurePending()
    {
        if (Status != ErasureRequestStatus.Requested)
            throw new InvalidOperationException("Only a requested erasure request can be reviewed.");
    }

    private void StampReview(string reviewedByUserId, string? note)
    {
        if (string.IsNullOrWhiteSpace(reviewedByUserId))
            throw new ArgumentException("A reviewer is required.", nameof(reviewedByUserId));
        ReviewedByUserId = reviewedByUserId;
        ReviewedAt = DateTime.UtcNow;
        ReviewNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }
}
