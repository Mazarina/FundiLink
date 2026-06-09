using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Common.Models;
using MediatR;

namespace FundiLink.Application.Features.Admin.Queries.GetAuditLog;

public class GetAuditLogHandler : IRequestHandler<GetAuditLogQuery, PagedResult<AuditLogEntryDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditLogHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<PagedResult<AuditLogEntryDto>> Handle(GetAuditLogQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 200 ? 50 : request.PageSize;

        var (items, total) = await _auditLogRepository.GetPagedAsync(page, pageSize, cancellationToken);

        var dtos = items.Select(e => new AuditLogEntryDto(
            e.Id, e.ActorUserId, e.ActorRole, e.Action, e.TargetType, e.TargetId, e.OccurredAt));

        return new PagedResult<AuditLogEntryDto>(dtos, total, page, pageSize);
    }
}
