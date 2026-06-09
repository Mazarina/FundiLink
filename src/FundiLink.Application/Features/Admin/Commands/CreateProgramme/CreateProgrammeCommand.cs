using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.CreateProgramme;

public record RequiredSubjectInput(string SubjectName, int MinimumPercentage);

public record CreateProgrammeCommand(
    string ActorUserId,
    string ActorRole,
    Guid InstitutionId,
    string Name,
    string? FacultyOrSchool,
    int? NfqLevel,
    int MinimumAps,
    List<RequiredSubjectInput> RequiredSubjects,
    DateTime? ApplicationOpenDate,
    DateTime? ApplicationCloseDate) : IRequest<Guid>;
