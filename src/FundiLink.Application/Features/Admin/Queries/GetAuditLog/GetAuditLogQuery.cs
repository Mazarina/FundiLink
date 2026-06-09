using FundiLink.Application.Common.Models;
using MediatR;

namespace FundiLink.Application.Features.Admin.Queries.GetAuditLog;

public record GetAuditLogQuery(int Page, int PageSize) : IRequest<PagedResult<AuditLogEntryDto>>;

public record AuditLogEntryDto(
    Guid Id,
    string ActorUserId,
    string ActorRole,
    string Action,
    string TargetType,
    string TargetId,
    DateTime OccurredAt);
