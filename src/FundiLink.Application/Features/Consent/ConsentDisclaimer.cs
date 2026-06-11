namespace FundiLink.Application.Features.Consent;

/// <summary>
/// Standard POPIA notice shown wherever guardian consent is captured or displayed.
/// </summary>
public static class ConsentDisclaimer
{
    public const string Text =
        "Where a learner is under 18, a parent or guardian's consent is required before FundiLink " +
        "processes or shares personal information. Consent is recorded, can be withdrawn at any time, " +
        "and a guardian's co-access is limited to exactly what consent permits. FundiLink does not " +
        "submit applications to any institution or funder on your behalf.";
}
