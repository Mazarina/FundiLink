using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Applications.Queries.GetMyApplications;

public record GetMyApplicationsQuery(string UserId) : IRequest<IEnumerable<ApplicationSummaryDto>>;

public record ApplicationSummaryDto(
    Guid Id,
    Guid ProgrammeId,
    string ProgrammeName,
    string InstitutionName,
    ApplicationStatus Status,
    DateTime? DeadlineDate,
    DateTime? SubmittedAt
);
