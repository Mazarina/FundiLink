using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.CreateInstitution;

public record CreateInstitutionCommand(
    string ActorUserId,
    string ActorRole,
    string Name,
    InstitutionType InstitutionType,
    string Province,
    string? Website) : IRequest<Guid>;
