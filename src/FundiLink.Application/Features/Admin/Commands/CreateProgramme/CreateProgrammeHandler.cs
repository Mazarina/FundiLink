using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.CreateProgramme;

public class CreateProgrammeHandler : IRequestHandler<CreateProgrammeCommand, Guid>
{
    private readonly IProgrammeRepository _programmeRepository;
    private readonly IInstitutionRepository _institutionRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public CreateProgrammeHandler(
        IProgrammeRepository programmeRepository,
        IInstitutionRepository institutionRepository,
        IAuditLogRepository auditLogRepository)
    {
        _programmeRepository = programmeRepository;
        _institutionRepository = institutionRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Guid> Handle(CreateProgrammeCommand request, CancellationToken cancellationToken)
    {
        _ = await _institutionRepository.GetByIdAsync(request.InstitutionId, cancellationToken)
            ?? throw new KeyNotFoundException("Institution not found.");

        var requiredSubjects = request.RequiredSubjects
            .Select(s => new RequiredSubject(s.SubjectName, s.MinimumPercentage));

        var programme = Programme.Create(
            request.InstitutionId,
            request.Name,
            request.MinimumAps,
            requiredSubjects,
            request.FacultyOrSchool,
            request.NfqLevel,
            request.ApplicationOpenDate,
            request.ApplicationCloseDate);

        await _programmeRepository.AddAsync(programme, cancellationToken);
        await _programmeRepository.SaveChangesAsync(cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(request.ActorUserId, request.ActorRole, "CreateProgramme", "Programme", programme.Id.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        return programme.Id;
    }
}
