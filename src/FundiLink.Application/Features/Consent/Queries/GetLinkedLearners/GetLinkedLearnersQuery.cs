using MediatR;

namespace FundiLink.Application.Features.Consent.Queries.GetLinkedLearners;

public record GetLinkedLearnersQuery(string GuardianUserId)
    : IRequest<IReadOnlyList<LinkedLearnerDto>>;

/// <summary>Minimised list entry of a learner a guardian is linked to.</summary>
public record LinkedLearnerDto(
    Guid LearnerId,
    string FirstName,
    string Surname,
    bool HasCurrentConsent
);
