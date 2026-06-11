using FundiLink.Application.Common.Models;
using FundiLink.Application.Features.Admin.Queries.GetAuditLog;
using MediatR;

namespace FundiLink.Application.Features.Reporting.Queries.GetAuditActivityReport;

/// <summary>
/// Filtered, paged report over the existing append-only audit log (by action, actor role,
/// date range). Reuses <see cref="AuditLogEntryDto"/>. SuperAdmin only (enforced at the API
/// boundary). Read-only — surfaces no learner PII beyond the existing audit fields.
/// </summary>
public record GetAuditActivityReportQuery(
    string? Action,
    string? ActorRole,
    DateTime? FromUtc,
    DateTime? ToUtc,
    int Page,
    int PageSize) : IRequest<PagedResult<AuditLogEntryDto>>;
