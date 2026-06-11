namespace FundiLink.Domain.Enums;

/// <summary>
/// The lifecycle of a learner-initiated erasure request (POPIA right to erasure).
/// A learner raises a request (Requested); an admin reviews it (Approved/Rejected);
/// approval is followed by a deliberate, audited fulfilment that anonymises personal
/// data (Fulfilled). Status transitions are append-only audit-logged.
/// </summary>
public enum ErasureRequestStatus
{
    Requested,
    Approved,
    Rejected,
    Fulfilled
}
