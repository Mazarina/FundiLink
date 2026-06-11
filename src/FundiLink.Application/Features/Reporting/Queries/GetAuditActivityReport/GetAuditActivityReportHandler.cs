using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Common.Models;
using FundiLink.Application.Features.Admin.Queries.GetAuditLog;
using MediatR;

namespace FundiLink.Application.Features.Reporting.Queries.GetAuditActivityReport;

public class GetAuditActivityReportHandler
    : IRequestHandler<GetAuditActivityReportQuery, PagedResult<AuditLogEntryDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditActivityReportHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<PagedResult<AuditLogEntryDto>> Handle(
        GetAuditActivityReportQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 200 ? 50 : request.PageSize;

        var (items, total) = await _auditLogRepository.GetFilteredAsync(
            request.Action, request.ActorRole, request.FromUtc, request.ToUtc,
            page, pageSize, cancellationToken);

        var dtos = items.Select(e => new AuditLogEntryDto(
            e.Id, e.ActorUserId, e.ActorRole, e.Action, e.TargetType, e.TargetId, e.OccurredAt));

        return new PagedResult<AuditLogEntryDto>(dtos, total, page, pageSize);
    }
}
