using MediatR;

namespace FundiLink.Application.Features.Home.Queries.GetLearnerHomeSummary;

/// <summary>
/// Owner-scoped request for the authenticated learner's home dashboard summary. The learner is
/// resolved from <paramref name="UserId"/>; no other learner's data is ever read.
/// <paramref name="DeadlineWindowDays"/> bounds the upcoming-deadline lookahead.
/// </summary>
public record GetLearnerHomeSummaryQuery(string UserId, int DeadlineWindowDays = 30)
    : IRequest<LearnerHomeSummaryDto>;
