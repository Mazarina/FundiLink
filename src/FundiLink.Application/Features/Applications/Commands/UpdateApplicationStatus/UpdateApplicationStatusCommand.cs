using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Applications.Commands.UpdateApplicationStatus;

public record UpdateApplicationStatusCommand(
    Guid ApplicationId,
    string UserId,
    ApplicationStatus NewStatus,
    string? Notes = null
) : IRequest;
