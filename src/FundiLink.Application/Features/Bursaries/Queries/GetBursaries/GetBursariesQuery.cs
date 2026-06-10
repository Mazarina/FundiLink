using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Bursaries.Queries.GetBursaries;

public record GetBursariesQuery(
    string? FieldOfStudy,
    string? Province,
    BursaryFundingType? FundingType
) : IRequest<IEnumerable<BursaryDto>>;

public record BursaryDto(
    Guid Id,
    string Name,
    string ProviderName,
    string Description,
    BursaryFundingType FundingType,
    IReadOnlyList<string> FieldsOfStudy,
    int? MinimumAps,
    decimal? MaxHouseholdIncome,
    IReadOnlyList<string> ProvincesEligible,
    DateTime? ApplicationOpenDate,
    DateTime? ApplicationCloseDate,
    string? ExternalApplicationUrl,
    string Disclaimer
);
