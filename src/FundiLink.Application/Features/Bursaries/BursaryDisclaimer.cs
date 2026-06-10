namespace FundiLink.Application.Features.Bursaries;

/// <summary>
/// Shared guidance-only disclaimer for all bursary-facing responses.
/// FundiLink is NOT an official bursary, NSFAS, or funding admissions platform.
/// </summary>
public static class BursaryDisclaimer
{
    public const string Text =
        "This is curated public information for guidance only. FundiLink is not an official " +
        "bursary, NSFAS, or funding platform and does not guarantee funding. Always apply on " +
        "the funder's official portal and verify details with the funder.";
}
