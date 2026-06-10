namespace FundiLink.Domain.Enums;

/// <summary>
/// The constrained set of questions the AI guidance assistant can answer.
/// MVP supports profile-aware, grounded intents only — no free-form open-domain chat.
/// </summary>
public enum AssistantIntent
{
    WhatDoIQualifyFor,
    WhatIsMyAps,
    WhatDocumentsDoINeed,
    WhichBursariesFitMe
}
