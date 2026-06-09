using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.CreateInstitution;

public class CreateInstitutionHandler : IRequestHandler<CreateInstitutionCommand, Guid>
{
    private readonly IInstitutionRepository _institutionRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public CreateInstitutionHandler(IInstitutionRepository institutionRepository, IAuditLogRepository auditLogRepository)
    {
        _institutionRepository = institutionRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Guid> Handle(CreateInstitutionCommand request, CancellationToken cancellationToken)
    {
        var institution = Institution.Create(request.Name, request.InstitutionType, request.Province, request.Website);
        await _institutionRepository.AddAsync(institution, cancellationToken);
        await _institutionRepository.SaveChangesAsync(cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(request.ActorUserId, request.ActorRole, "CreateInstitution", "Institution", institution.Id.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        return institution.Id;
    }
}
