using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.UpdateInstitution;

public class UpdateInstitutionHandler : IRequestHandler<UpdateInstitutionCommand>
{
    private readonly IInstitutionRepository _institutionRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public UpdateInstitutionHandler(IInstitutionRepository institutionRepository, IAuditLogRepository auditLogRepository)
    {
        _institutionRepository = institutionRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task Handle(UpdateInstitutionCommand request, CancellationToken cancellationToken)
    {
        var institution = await _institutionRepository.GetByIdAsync(request.InstitutionId, cancellationToken)
            ?? throw new KeyNotFoundException("Institution not found.");

        institution.Update(request.Name, request.InstitutionType, request.Province, request.Website, request.IsActive);
        await _institutionRepository.SaveChangesAsync(cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(request.ActorUserId, request.ActorRole, "UpdateInstitution", "Institution", request.InstitutionId.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);
    }
}
