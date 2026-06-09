using FundiLink.Application.Common.Models;
using MediatR;

namespace FundiLink.Application.Features.Admin.Queries.GetLearnerOverview;

public record GetLearnerOverviewQuery(
    string ActorUserId,
    string ActorRole,
    Guid LearnerId) : IRequest<LearnerOverview>;
