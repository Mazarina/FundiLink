using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Applications.Queries.GetApplicationById;

public record GetApplicationByIdQuery(Guid ApplicationId, string UserId) : IRequest<ApplicationDetailDto>;

public record ApplicationDetailDto(
    Guid Id,
    Guid ProgrammeId,
    string ProgrammeName,
    string InstitutionName,
    ApplicationStatus Status,
    string? Notes,
    DateTime? DeadlineDate,
    DateTime? SubmittedAt,
    DateTime? OutcomeAt
);
