namespace FundiLink.Application.Features.Accommodation;

/// <summary>
/// Shared guidance-only disclaimer for all accommodation-facing responses.
/// FundiLink is NOT a landlord, accommodation provider, or booking agent.
/// </summary>
public static class AccommodationDisclaimer
{
    public const string Text =
        "This is curated public information for guidance only. FundiLink is not an accommodation " +
        "provider, landlord, or booking agent and does not guarantee availability, price, or safety. " +
        "Always verify details directly with the provider or talk to a FundiLink support agent before committing.";
}
