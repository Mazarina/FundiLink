using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Enums;
using FundiLink.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace FundiLink.Application.Tests.Features.Assistant;

public class RuleBasedAiAssistantServiceTests
{
    private readonly RuleBasedAiAssistantService _sut = new(NullLogger<RuleBasedAiAssistantService>.Instance);

    private static AssistantRequest BuildRequest(
        AssistantIntent intent,
        int? aps = 38,
        IReadOnlyList<ProgrammeMatchContext>? programmes = null,
        IReadOnlyList<BursaryMatchContext>? bursaries = null,
        IReadOnlyList<string>? documents = null)
        => new(
            intent,
            "Thabo",
            aps,
            "Gauteng",
            programmes ?? [],
            bursaries ?? [],
            documents ?? []);

    [Fact]
    public async Task WhatIsMyAps_WithScore_ReturnsGroundedScore()
    {
        var result = await _sut.AnswerAsync(BuildRequest(AssistantIntent.WhatIsMyAps, aps: 41));

        result.Answer.Should().Contain("41");
        result.Answer.Should().Contain("Thabo");
        result.Sources.Should().NotBeEmpty();
    }

    [Fact]
    public async Task WhatIsMyAps_NoScore_GuidesToAddProfile()
    {
        var result = await _sut.AnswerAsync(BuildRequest(AssistantIntent.WhatIsMyAps, aps: null));

        result.Answer.Should().Contain("not calculated your APS");
    }

    [Fact]
    public async Task WhatDoIQualifyFor_UsesEligibleProgrammesOnly()
    {
        var programmes = new List<ProgrammeMatchContext>
        {
            new("BSc Computer Science", "Wits", 38, IsEligible: true, ApsGap: 0),
            new("BCom", "UJ", 42, IsEligible: false, ApsGap: 4)
        };

        var result = await _sut.AnswerAsync(
            BuildRequest(AssistantIntent.WhatDoIQualifyFor, aps: 38, programmes: programmes));

        result.Answer.Should().Contain("BSc Computer Science");
        result.Answer.Should().Contain("Wits");
        result.Answer.Should().NotContain("BCom");
    }

    [Fact]
    public async Task WhichBursariesFitMe_ListsMatches_OrSafeNoMatchMessage()
    {
        var bursaries = new List<BursaryMatchContext>
        {
            new("Future Leaders", "ZulTek Foundation", 35)
        };

        var withMatch = await _sut.AnswerAsync(
            BuildRequest(AssistantIntent.WhichBursariesFitMe, bursaries: bursaries));
        withMatch.Answer.Should().Contain("Future Leaders");

        var noMatch = await _sut.AnswerAsync(BuildRequest(AssistantIntent.WhichBursariesFitMe));
        noMatch.Answer.Should().Contain("No bursaries");
    }

    [Fact]
    public async Task WhatDocumentsDoINeed_ReportsMissingCoreDocuments()
    {
        var uploaded = new List<string> { DocumentType.IdDocument.ToString() };

        var result = await _sut.AnswerAsync(
            BuildRequest(AssistantIntent.WhatDocumentsDoINeed, documents: uploaded));

        result.Answer.Should().Contain("matric certificate");
        result.Answer.Should().NotContain("ID document,"); // ID already uploaded
        result.Sources.Should().Contain("Your FundiLink uploaded documents");
    }
}
