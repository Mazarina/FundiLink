namespace FundiLink.Application.Features.Assistant;

/// <summary>
/// Shared guidance-only disclaimer for all AI assistant responses.
/// FundiLink is NOT an official advisor. Assistant output is guidance only and is
/// grounded strictly in the learner's own FundiLink data — it never fabricates
/// institution, programme, bursary, or NSFAS facts.
/// </summary>
public static class AssistantDisclaimer
{
    public const string Text =
        "This is automated guidance only, based on the information in your FundiLink profile. " +
        "FundiLink is not an official university, TVET, NSFAS, or bursary advisor and does not " +
        "guarantee admission or funding. Always verify requirements, deadlines, and amounts with " +
        "the official institution or funder, or talk to a FundiLink support agent.";
}
