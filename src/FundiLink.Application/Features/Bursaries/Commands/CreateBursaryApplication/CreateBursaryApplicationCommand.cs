using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Bursaries.Commands.CreateBursaryApplication;

public record CreateBursaryApplicationCommand(
    Guid BursaryId,
    string UserId,
    BursaryApplicationStatus Status = BursaryApplicationStatus.Researching,
    string? Notes = null,
    DateTime? DeadlineDate = null
) : IRequest<Guid>;
