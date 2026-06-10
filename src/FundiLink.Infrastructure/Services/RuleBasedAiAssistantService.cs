using System.Text;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FundiLink.Infrastructure.Services;

/// <summary>
/// Deterministic, rule/template-based implementation of <see cref="IAiAssistantService"/>.
/// No external LLM call is made — answers are composed strictly from the learner's own
/// FundiLink data passed in the request. This guarantees no fabricated institution,
/// programme, bursary, or NSFAS facts. A real provider may be wired later behind this
/// same interface, with any API key supplied via environment variables only.
/// </summary>
public class RuleBasedAiAssistantService : IAiAssistantService
{
    // The recommended set of documents a learner should prepare for applications.
    private static readonly DocumentType[] RecommendedDocuments =
    [
        DocumentType.IdDocument,
        DocumentType.MatricCertificate,
        DocumentType.AcademicResults,
        DocumentType.ProofOfResidence
    ];

    private readonly ILogger<RuleBasedAiAssistantService> _logger;

    public RuleBasedAiAssistantService(ILogger<RuleBasedAiAssistantService> logger)
    {
        _logger = logger;
    }

    public Task<AssistantResult> AnswerAsync(AssistantRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[Assistant] Answering intent {Intent} (rule-based, no external call).", request.Intent);

        var result = request.Intent switch
        {
            AssistantIntent.WhatIsMyAps => AnswerAps(request),
            AssistantIntent.WhatDoIQualifyFor => AnswerQualify(request),
            AssistantIntent.WhichBursariesFitMe => AnswerBursaries(request),
            AssistantIntent.WhatDocumentsDoINeed => AnswerDocuments(request),
            _ => new AssistantResult(
                "I can only help with a fixed set of guidance questions right now.", [])
        };

        return Task.FromResult(result);
    }

    private static AssistantResult AnswerAps(AssistantRequest request)
    {
        var sb = new StringBuilder();
        sb.Append($"Hi {request.FirstName}. ");
        if (request.ApsScore is null)
        {
            sb.Append("You have not calculated your APS yet. Add your subjects and marks in your ");
            sb.Append("academic profile to get an APS estimate.");
            return new AssistantResult(sb.ToString(), ["Your FundiLink academic profile"]);
        }

        sb.Append($"Based on your FundiLink academic profile, your estimated APS is {request.ApsScore}. ");
        sb.Append("This is an estimate from the marks you entered — official APS is confirmed by each institution.");
        return new AssistantResult(sb.ToString(), ["Your FundiLink academic profile (APS estimate)"]);
    }

    private static AssistantResult AnswerQualify(AssistantRequest request)
    {
        if (request.ApsScore is null)
        {
            return new AssistantResult(
                $"Hi {request.FirstName}. I cannot estimate what you qualify for yet because your APS " +
                "is not calculated. Add your subjects and marks to your academic profile first.",
                ["Your FundiLink academic profile"]);
        }

        var eligible = request.ProgrammeMatches.Where(p => p.IsEligible).ToList();
        var nearMiss = request.ProgrammeMatches
            .Where(p => !p.IsEligible)
            .OrderBy(p => p.ApsGap)
            .Take(3)
            .ToList();

        var sb = new StringBuilder();
        sb.Append($"Hi {request.FirstName}. With an estimated APS of {request.ApsScore}, ");

        if (eligible.Count == 0)
        {
            sb.Append("you do not yet meet the requirements for the programmes currently in FundiLink. ");
            if (nearMiss.Count > 0)
            {
                sb.Append("The closest ones are: ");
                sb.Append(string.Join("; ", nearMiss.Select(p =>
                    $"{p.ProgrammeName} at {p.InstitutionName} (needs APS {p.MinimumAps}, you are {p.ApsGap} short)")));
                sb.Append('.');
            }
        }
        else
        {
            sb.Append($"you may qualify for {eligible.Count} programme(s) listed in FundiLink, including: ");
            sb.Append(string.Join("; ", eligible.Take(5).Select(p =>
                $"{p.ProgrammeName} at {p.InstitutionName} (APS {p.MinimumAps})")));
            sb.Append(". \"May qualify\" is not a guarantee — confirm requirements on each institution's official portal.");
        }

        return new AssistantResult(sb.ToString(),
            ["Your FundiLink academic profile (APS estimate)", "FundiLink programme matches"]);
    }

    private static AssistantResult AnswerBursaries(AssistantRequest request)
    {
        var sb = new StringBuilder();
        sb.Append($"Hi {request.FirstName}. ");

        if (request.BursaryMatches.Count == 0)
        {
            sb.Append("No bursaries in FundiLink currently match your profile. Keep your academic profile ");
            sb.Append("up to date, as new opportunities are added regularly.");
        }
        else
        {
            sb.Append($"Based on your profile, {request.BursaryMatches.Count} bursary/bursaries in FundiLink may fit you: ");
            sb.Append(string.Join("; ", request.BursaryMatches.Take(5).Select(b =>
                b.MinimumAps is null
                    ? $"{b.Name} by {b.ProviderName}"
                    : $"{b.Name} by {b.ProviderName} (min APS {b.MinimumAps})")));
            sb.Append(". Apply on each funder's official portal — FundiLink does not guarantee funding.");
        }

        return new AssistantResult(sb.ToString(), ["Your FundiLink profile", "FundiLink bursary matches"]);
    }

    private static AssistantResult AnswerDocuments(AssistantRequest request)
    {
        var missing = RecommendedDocuments
            .Where(d => !request.UploadedDocumentTypes.Contains(d.ToString()))
            .ToList();

        var sb = new StringBuilder();
        sb.Append($"Hi {request.FirstName}. ");

        if (missing.Count == 0)
        {
            sb.Append("You have uploaded all of the core documents FundiLink recommends ");
            sb.Append("(ID, matric certificate, academic results, and proof of residence). ");
            sb.Append("Some institutions or funders may ask for extra documents — check each one's requirements.");
        }
        else
        {
            sb.Append("Based on what you have uploaded, you still need to prepare: ");
            sb.Append(string.Join(", ", missing.Select(FriendlyDocumentName)));
            sb.Append(". These are commonly required by institutions and funders.");
        }

        return new AssistantResult(sb.ToString(), ["Your FundiLink uploaded documents"]);
    }

    private static string FriendlyDocumentName(DocumentType type) => type switch
    {
        DocumentType.IdDocument => "ID document",
        DocumentType.MatricCertificate => "matric certificate",
        DocumentType.AcademicResults => "academic results",
        DocumentType.ProofOfResidence => "proof of residence",
        DocumentType.GuardianConsent => "guardian consent",
        _ => type.ToString()
    };
}
