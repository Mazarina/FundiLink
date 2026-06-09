using FluentAssertions;
using FundiLink.Application.Features.AcademicProfile.Services;
using FundiLink.Domain.Entities;

namespace FundiLink.Application.Tests.Features.AcademicProfile;

public class ApsCalculatorServiceTests
{
    private readonly ApsCalculatorService _sut = new();
    private static readonly Guid ProfileId = Guid.NewGuid();

    private static NscSubjectResult Subject(string name, int pct, bool isLo = false)
        => NscSubjectResult.Create(ProfileId, name, pct, isLifeOrientation: isLo);

    // ── APS point conversion ─────────────────────────────────

    [Theory]
    [InlineData(80, 7)]
    [InlineData(100, 7)]
    [InlineData(79, 6)]
    [InlineData(70, 6)]
    [InlineData(69, 5)]
    [InlineData(60, 5)]
    [InlineData(59, 4)]
    [InlineData(50, 4)]
    [InlineData(49, 3)]
    [InlineData(40, 3)]
    [InlineData(39, 2)]
    [InlineData(30, 2)]
    [InlineData(29, 1)]
    [InlineData(0, 1)]
    public void PercentageToPoints_CorrectScale(int percentage, int expectedPoints)
    {
        ApsCalculatorService.PercentageToApsPoints(percentage).Should().Be(expectedPoints);
    }

    // ── Life Orientation exclusion ───────────────────────────

    [Fact]
    public void CalculateAps_ExcludesLifeOrientation()
    {
        var subjects = new[]
        {
            Subject("Mathematics", 80),        // 7
            Subject("English", 70),            // 6
            Subject("Physical Sciences", 65),  // 5
            Subject("Life Sciences", 60),      // 5
            Subject("Geography", 55),          // 4
            Subject("Accounting", 50),         // 4
            Subject("Life Orientation", 90, isLo: true), // should be excluded
        };

        _sut.CalculateAps(subjects).Should().Be(31); // 7+6+5+5+4+4
    }

    [Fact]
    public void CalculateAps_WithOnlyLifeOrientation_ReturnsZero()
    {
        var subjects = new[] { Subject("Life Orientation", 90, isLo: true) };
        _sut.CalculateAps(subjects).Should().Be(0);
    }

    // ── Top 6 selection ──────────────────────────────────────

    [Fact]
    public void CalculateAps_TakesTop6SubjectsExcludingLo()
    {
        var subjects = new[]
        {
            Subject("Mathematics", 80),       // 7
            Subject("English", 75),           // 6
            Subject("Physical Sciences", 65), // 5
            Subject("Life Sciences", 60),     // 5
            Subject("Geography", 55),         // 4
            Subject("Accounting", 50),        // 4
            Subject("History", 40),           // 3  ← 7th, should be dropped
            Subject("Life Orientation", 90, isLo: true), // excluded
        };

        _sut.CalculateAps(subjects).Should().Be(31); // top 6: 7+6+5+5+4+4
    }

    [Fact]
    public void CalculateAps_FewerThan6Subjects_SumsAll()
    {
        var subjects = new[]
        {
            Subject("Mathematics", 80), // 7
            Subject("English", 70),     // 6
        };

        _sut.CalculateAps(subjects).Should().Be(13);
    }

    // ── Boundary values ──────────────────────────────────────

    [Fact]
    public void CalculateAps_AllMaxScores_Returns42()
    {
        var subjects = Enumerable.Range(1, 6)
            .Select(i => Subject($"Subject{i}", 100))
            .ToArray();

        _sut.CalculateAps(subjects).Should().Be(42); // 6 × 7
    }

    [Fact]
    public void CalculateAps_AllMinScores_Returns6()
    {
        var subjects = Enumerable.Range(1, 6)
            .Select(i => Subject($"Subject{i}", 0))
            .ToArray();

        _sut.CalculateAps(subjects).Should().Be(6); // 6 × 1
    }

    [Fact]
    public void CalculateAps_EmptySubjects_ReturnsZero()
    {
        _sut.CalculateAps([]).Should().Be(0);
    }

    [Fact]
    public void CalculateAps_ExactBoundary80Percent_Returns7Points()
    {
        var subjects = new[] { Subject("Mathematics", 80) };
        _sut.CalculateAps(subjects).Should().Be(7);
    }

    [Fact]
    public void CalculateAps_ExactBoundary30Percent_Returns2Points()
    {
        var subjects = new[] { Subject("History", 30) };
        _sut.CalculateAps(subjects).Should().Be(2);
    }

    // ── Typical learner scenario ─────────────────────────────

    [Fact]
    public void CalculateAps_TypicalMatricResult_CorrectTotal()
    {
        // Thabo's results: Maths 65, English 72, Physical Sci 58, LifeSci 61, Geography 55, History 48, LO 80
        var subjects = new[]
        {
            Subject("Mathematics", 65),         // 5
            Subject("English Home Language", 72), // 6
            Subject("Physical Sciences", 58),   // 4
            Subject("Life Sciences", 61),        // 5
            Subject("Geography", 55),            // 4
            Subject("History", 48),              // 3
            Subject("Life Orientation", 80, isLo: true), // excluded
        };

        _sut.CalculateAps(subjects).Should().Be(27); // 5+6+4+5+4+3
    }
}
