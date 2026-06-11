using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Consent.Queries.GetConsentState;

public record GetConsentStateQuery(string UserId) : IRequest<ConsentStateDto>;

/// <summary>The effective consent state for the caller's own learner profile.</summary>
public record ConsentStateDto(
    bool IsMinor,
    bool GuardianConsentRequired,
    IReadOnlyList<ConsentTypeStateDto> Consents,
    string Disclaimer
);

public record ConsentTypeStateDto(
    ConsentType ConsentType,
    bool IsGranted,
    ConsentScope? Scope,
    string? GuardianName,
    DateTime? RecordedAt
);
