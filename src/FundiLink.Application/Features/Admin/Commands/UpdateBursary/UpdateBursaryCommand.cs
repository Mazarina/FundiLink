using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.UpdateBursary;

public record UpdateBursaryCommand(
    string ActorUserId,
    string ActorRole,
    Guid BursaryId,
    string Name,
    string ProviderName,
    string Description,
    BursaryFundingType FundingType,
    List<string> FieldsOfStudy,
    int? MinimumAps,
    decimal? MaxHouseholdIncome,
    List<string> ProvincesEligible,
    DateTime? ApplicationOpenDate,
    DateTime? ApplicationCloseDate,
    string? ExternalApplicationUrl,
    bool IsActive) : IRequest;
