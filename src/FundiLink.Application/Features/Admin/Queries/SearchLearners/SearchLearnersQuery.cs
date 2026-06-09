using FundiLink.Application.Common.Models;
using MediatR;

namespace FundiLink.Application.Features.Admin.Queries.SearchLearners;

public record SearchLearnersQuery(
    string ActorUserId,
    string ActorRole,
    string? Keyword,
    string? Province,
    int Page,
    int PageSize) : IRequest<PagedResult<LearnerSummary>>;
