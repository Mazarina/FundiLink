using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.CreateBursary;

public record CreateBursaryCommand(
    string ActorUserId,
    string ActorRole,
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
    string? ExternalApplicationUrl) : IRequest<Guid>;
