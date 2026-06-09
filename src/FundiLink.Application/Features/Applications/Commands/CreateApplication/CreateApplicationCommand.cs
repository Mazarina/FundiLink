using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Applications.Commands.CreateApplication;

public record CreateApplicationCommand(
    Guid ProgrammeId,
    string UserId,
    ApplicationStatus Status = ApplicationStatus.Interested,
    string? Notes = null,
    DateTime? DeadlineDate = null
) : IRequest<Guid>;
