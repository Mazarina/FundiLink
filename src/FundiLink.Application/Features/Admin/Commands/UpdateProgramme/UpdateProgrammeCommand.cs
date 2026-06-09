using FundiLink.Application.Features.Admin.Commands.CreateProgramme;
using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.UpdateProgramme;

public record UpdateProgrammeCommand(
    string ActorUserId,
    string ActorRole,
    Guid ProgrammeId,
    string Name,
    string? FacultyOrSchool,
    int? NfqLevel,
    int MinimumAps,
    List<RequiredSubjectInput> RequiredSubjects,
    DateTime? ApplicationOpenDate,
    DateTime? ApplicationCloseDate,
    bool IsActive) : IRequest;
