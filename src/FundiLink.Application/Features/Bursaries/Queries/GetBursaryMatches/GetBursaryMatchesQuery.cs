using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Bursaries.Queries.GetBursaryMatches;

public record GetBursaryMatchesQuery(string UserId) : IRequest<IEnumerable<BursaryMatchDto>>;

public record BursaryMatchDto(
    Guid BursaryId,
    string Name,
    string ProviderName,
    BursaryFundingType FundingType,
    int? MinimumAps,
    string? ExternalApplicationUrl,
    IReadOnlyList<string> Reasons,
    bool GuidanceOnly,
    string Disclaimer
);
