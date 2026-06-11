namespace FundiLink.Domain.Enums;

/// <summary>
/// The state captured by an append-only guardian consent record.
/// The effective consent state is derived from the most recent record per
/// (learner, consent type) — a Revoked record supersedes an earlier Granted one.
/// </summary>
public enum ConsentStatus
{
    Granted,
    Revoked
}
