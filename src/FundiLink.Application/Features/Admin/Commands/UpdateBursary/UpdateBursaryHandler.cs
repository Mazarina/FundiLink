using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.UpdateBursary;

public class UpdateBursaryHandler : IRequestHandler<UpdateBursaryCommand>
{
    private readonly IBursaryRepository _bursaryRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public UpdateBursaryHandler(IBursaryRepository bursaryRepository, IAuditLogRepository auditLogRepository)
    {
        _bursaryRepository = bursaryRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task Handle(UpdateBursaryCommand request, CancellationToken cancellationToken)
    {
        var bursary = await _bursaryRepository.GetByIdAsync(request.BursaryId, cancellationToken)
            ?? throw new KeyNotFoundException("Bursary not found.");

        bursary.Update(
            request.Name,
            request.ProviderName,
            request.Description,
            request.FundingType,
            request.FieldsOfStudy,
            request.MinimumAps,
            request.MaxHouseholdIncome,
            request.ProvincesEligible,
            request.ApplicationOpenDate,
            request.ApplicationCloseDate,
            request.ExternalApplicationUrl,
            request.IsActive);

        await _bursaryRepository.SaveChangesAsync(cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(request.ActorUserId, request.ActorRole, "UpdateBursary", "Bursary", request.BursaryId.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);
    }
}
