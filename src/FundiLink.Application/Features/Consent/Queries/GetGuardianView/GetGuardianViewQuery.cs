using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Consent.Queries.GetGuardianView;

/// <summary>
/// A guardian requesting the consent-gated, minimised read-only view of a linked
/// minor learner. GuardianUserId is the authenticated caller; LearnerId is the target.
/// </summary>
public record GetGuardianViewQuery(string GuardianUserId, Guid LearnerId)
    : IRequest<GuardianViewDto>;

/// <summary>
/// Minimised, read-only learner view returned to a consented guardian. Contains only
/// the fields permitted by the consent scope — never the ID number, documents, or
/// guardian/contact details of the learner (data minimisation per POPIA).
/// </summary>
public record GuardianViewDto(
    Guid LearnerId,
    string FirstName,
    string Surname,
    GradeLevel GradeLevel,
    string SchoolName,
    string Province,
    int ProfileCompleteness,
    ConsentScope Scope,
    IReadOnlyList<GuardianApplicationSummaryDto> Applications,
    string Disclaimer
);

/// <summary>Minimised application summary — status only, no notes or sensitive detail.</summary>
public record GuardianApplicationSummaryDto(
    string ProgrammeOrBursaryName,
    string Kind,
    string Status
);
