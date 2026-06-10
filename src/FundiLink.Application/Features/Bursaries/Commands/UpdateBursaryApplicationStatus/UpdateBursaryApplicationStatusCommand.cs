using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Bursaries.Commands.UpdateBursaryApplicationStatus;

public record UpdateBursaryApplicationStatusCommand(
    Guid BursaryApplicationId,
    string UserId,
    BursaryApplicationStatus NewStatus,
    string? Notes
) : IRequest;
