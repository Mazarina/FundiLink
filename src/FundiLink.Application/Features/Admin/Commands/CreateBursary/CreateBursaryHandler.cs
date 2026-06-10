using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.CreateBursary;

public class CreateBursaryHandler : IRequestHandler<CreateBursaryCommand, Guid>
{
    private readonly IBursaryRepository _bursaryRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public CreateBursaryHandler(IBursaryRepository bursaryRepository, IAuditLogRepository auditLogRepository)
    {
        _bursaryRepository = bursaryRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Guid> Handle(CreateBursaryCommand request, CancellationToken cancellationToken)
    {
        var bursary = Bursary.Create(
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
            request.ExternalApplicationUrl);

        await _bursaryRepository.AddAsync(bursary, cancellationToken);
        await _bursaryRepository.SaveChangesAsync(cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(request.ActorUserId, request.ActorRole, "CreateBursary", "Bursary", bursary.Id.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        return bursary.Id;
    }
}
