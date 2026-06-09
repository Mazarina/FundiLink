using FluentAssertions;
using FundiLink.Application.Features.ProgrammeMatching.Services;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;

namespace FundiLink.Application.Tests.Features.ProgrammeMatching;

public class ProgrammeMatchingServiceTests
{
    private readonly ProgrammeMatchingService _sut = new();

    private static Domain.Entities.AcademicProfile BuildProfile(int apsScore, params (string Name, int Pct, bool IsLo)[] subjects)
    {
        var profile = Domain.Entities.AcademicProfile.Create(Guid.NewGuid(), 2025, ResultType.Grade12Final);
        var results = subjects.Select(s =>
            NscSubjectResult.Create(profile.Id, s.Name, s.Pct, isLifeOrientation: s.IsLo)).ToList();
        profile.SetSubjects(results);
        profile.SetApsScore(apsScore);
        return profile;
    }

    private static (Programme, string) BuildProgramme(int minAps, params RequiredSubject[] required)
    {
        var institutionId = Guid.NewGuid();
        var programme = Programme.Create(institutionId, "Test Programme", minAps, required);
        return (programme, "Test University");
    }

    [Fact]
    public void Eligible_WhenApsMetAndAllRequiredSubjectsMet()
    {
        var profile = BuildProfile(40, ("Mathematics", 75, false), ("English Home Language", 65, false));
        var programmes = new[] { BuildProgramme(35, new RequiredSubject("Mathematics", 70)) };

        var result = _sut.GetMatchingProgrammes(profile, programmes).Single();

        result.IsEligible.Should().BeTrue();
        result.ApsGap.Should().Be(0);
        result.MissingSubjects.Should().BeEmpty();
    }

    [Fact]
    public void Ineligible_WhenApsTooLow()
    {
        var profile = BuildProfile(30, ("Mathematics", 75, false));
        var programmes = new[] { BuildProgramme(42, new RequiredSubject("Mathematics", 70)) };

        var result = _sut.GetMatchingProgrammes(profile, programmes).Single();

        result.IsEligible.Should().BeFalse();
        result.ApsGap.Should().Be(12);
    }

    [Fact]
    public void Ineligible_WhenRequiredSubjectMissingEntirely()
    {
        var profile = BuildProfile(45, ("English Home Language", 80, false));
        var programmes = new[] { BuildProgramme(40, new RequiredSubject("Mathematics", 60)) };

        var result = _sut.GetMatchingProgrammes(profile, programmes).Single();

        result.IsEligible.Should().BeFalse();
        result.MissingSubjects.Should().Contain("Mathematics");
    }

    [Fact]
    public void Ineligible_WhenRequiredSubjectPercentageTooLow()
    {
        var profile = BuildProfile(45, ("Mathematics", 50, false));
        var programmes = new[] { BuildProgramme(40, new RequiredSubject("Mathematics", 70)) };

        var result = _sut.GetMatchingProgrammes(profile, programmes).Single();

        result.IsEligible.Should().BeFalse();
        result.MissingSubjects.Should().Contain("Mathematics");
    }

    [Fact]
    public void EmptyRequiredSubjects_OnlyApsChecked()
    {
        var profile = BuildProfile(20, ("Mathematics", 30, false));
        var programmes = new[] { BuildProgramme(18) };

        var result = _sut.GetMatchingProgrammes(profile, programmes).Single();

        result.IsEligible.Should().BeTrue();
        result.MissingSubjects.Should().BeEmpty();
    }

    [Fact]
    public void MultipleProgrammes_FiltersCorrectly()
    {
        var profile = BuildProfile(38, ("Mathematics", 65, false), ("English Home Language", 60, false));
        var programmes = new[]
        {
            BuildProgramme(35, new RequiredSubject("Mathematics", 60)),  // eligible
            BuildProgramme(48, new RequiredSubject("Mathematics", 60)),  // aps too low
            BuildProgramme(35, new RequiredSubject("Physical Sciences", 60)) // missing subject
        };

        var results = _sut.GetMatchingProgrammes(profile, programmes).ToList();

        results.Count(r => r.IsEligible).Should().Be(1);
    }

    [Fact]
    public void LifeOrientation_NotConsideredAsRequiredSubjectBlocker()
    {
        // Learner does not have Life Orientation, but it must never block eligibility.
        var profile = BuildProfile(40, ("Mathematics", 75, false));
        var programmes = new[]
        {
            BuildProgramme(35, new RequiredSubject("Mathematics", 70), new RequiredSubject("Life Orientation", 60))
        };

        var result = _sut.GetMatchingProgrammes(profile, programmes).Single();

        result.IsEligible.Should().BeTrue();
        result.MissingSubjects.Should().NotContain("Life Orientation");
    }

    [Fact]
    public void AllProgrammesIneligible_WhenProfileHasZeroAps()
    {
        var profile = BuildProfile(0);
        var programmes = new[]
        {
            BuildProgramme(18),
            BuildProgramme(35, new RequiredSubject("Mathematics", 60))
        };

        var results = _sut.GetMatchingProgrammes(profile, programmes).ToList();

        results.Should().OnlyContain(r => !r.IsEligible);
    }
}
