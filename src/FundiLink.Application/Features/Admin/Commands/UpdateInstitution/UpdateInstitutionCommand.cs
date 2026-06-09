using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.UpdateInstitution;

public record UpdateInstitutionCommand(
    string ActorUserId,
    string ActorRole,
    Guid InstitutionId,
    string Name,
    InstitutionType InstitutionType,
    string Province,
    string? Website,
    bool IsActive) : IRequest;
