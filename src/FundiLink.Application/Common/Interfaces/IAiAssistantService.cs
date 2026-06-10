using FundiLink.Domain.Enums;

namespace FundiLink.Application.Common.Interfaces;

/// <summary>
/// Generates guidance-only answers grounded strictly in a learner's own FundiLink data.
/// No external LLM call in the MVP — implemented by a deterministic, rule-based stub.
/// A real provider may be wired later behind this same interface (key via environment only).
/// </summary>
public interface IAiAssistantService
{
    Task<AssistantResult> AnswerAsync(AssistantRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Grounded context assembled from the learner's existing FundiLink data.
/// </summary>
public record AssistantRequest(
    AssistantIntent Intent,
    string FirstName,
    int? ApsScore,
    string Province,
    IReadOnlyList<ProgrammeMatchContext> ProgrammeMatches,
    IReadOnlyList<BursaryMatchContext> BursaryMatches,
    IReadOnlyList<string> UploadedDocumentTypes);

public record ProgrammeMatchContext(string ProgrammeName, string InstitutionName, int MinimumAps, bool IsEligible, int ApsGap);

public record BursaryMatchContext(string Name, string ProviderName, int? MinimumAps);

/// <summary>
/// Typed assistant output. <see cref="Sources"/> lists which FundiLink data was used.
/// </summary>
public record AssistantResult(
    string Answer,
    IReadOnlyList<string> Sources);
