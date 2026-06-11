using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Consent.Queries.GetConsentHistory;

public record GetConsentHistoryQuery(string UserId) : IRequest<IReadOnlyList<ConsentHistoryEntryDto>>;

public record ConsentHistoryEntryDto(
    Guid Id,
    ConsentType ConsentType,
    ConsentScope Scope,
    ConsentStatus Status,
    string GuardianName,
    DateTime RecordedAt
);
