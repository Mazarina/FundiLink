namespace FundiLink.Application.Common.Models;

public record LearnerSummary(
    Guid Id,
    string FullName,
    string Email,
    string Province,
    string GradeLevel,
    int ProfileCompleteness);

public record LearnerOverview(
    LearnerSummary Summary,
    int ApsScore,
    int ApplicationCount,
    int DocumentCount);
