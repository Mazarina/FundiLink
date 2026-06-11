using FundiLink.Domain.Entities;

namespace FundiLink.Application.Common.Interfaces;

/// <summary>
/// Repository for learner-initiated erasure requests (POPIA right to erasure).
/// Requests are tracked through their lifecycle (Requested -> Approved/Rejected -> Fulfilled);
/// every transition is also recorded in the append-only audit log.
/// </summary>
public interface IErasureRequestRepository
{
    Task AddAsync(ErasureRequest request, CancellationToken ct);
    Task<ErasureRequest?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<ErasureRequest>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct);

    /// <summary>The learner's most recent open (Requested or Approved) request, or null.</summary>
    Task<ErasureRequest?> GetOpenByLearnerIdAsync(Guid learnerId, CancellationToken ct);

    /// <summary>All requests awaiting admin action (Requested), newest first.</summary>
    Task<IReadOnlyList<ErasureRequest>> GetPendingAsync(CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}
