using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Bursaries.Queries.GetMyBursaryApplications;

public record GetMyBursaryApplicationsQuery(string UserId) : IRequest<IEnumerable<BursaryApplicationSummaryDto>>;

public record BursaryApplicationSummaryDto(
    Guid Id,
    Guid BursaryId,
    string BursaryName,
    string ProviderName,
    BursaryApplicationStatus Status,
    string? Notes,
    DateTime? DeadlineDate,
    string? ExternalApplicationUrl,
    string Disclaimer
);
