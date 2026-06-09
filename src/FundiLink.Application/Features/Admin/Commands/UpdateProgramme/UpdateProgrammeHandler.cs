using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.UpdateProgramme;

public class UpdateProgrammeHandler : IRequestHandler<UpdateProgrammeCommand>
{
    private readonly IProgrammeRepository _programmeRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public UpdateProgrammeHandler(IProgrammeRepository programmeRepository, IAuditLogRepository auditLogRepository)
    {
        _programmeRepository = programmeRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task Handle(UpdateProgrammeCommand request, CancellationToken cancellationToken)
    {
        var programme = await _programmeRepository.GetByIdAsync(request.ProgrammeId, cancellationToken)
            ?? throw new KeyNotFoundException("Programme not found.");

        var requiredSubjects = request.RequiredSubjects
            .Select(s => new RequiredSubject(s.SubjectName, s.MinimumPercentage));

        programme.Update(
            request.Name,
            request.MinimumAps,
            requiredSubjects,
            request.FacultyOrSchool,
            request.NfqLevel,
            request.ApplicationOpenDate,
            request.ApplicationCloseDate,
            request.IsActive);

        await _programmeRepository.SaveChangesAsync(cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(request.ActorUserId, request.ActorRole, "UpdateProgramme", "Programme", request.ProgrammeId.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);
    }
}
