using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Bursaries.Queries.GetBursaryMatches;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Bursaries;

public class GetBursaryMatchesHandlerTests
{
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<IBursaryRepository> _bursaryRepository = new();
    private readonly GetBursaryMatchesHandler _sut;

    public GetBursaryMatchesHandlerTests()
    {
        _sut = new GetBursaryMatchesHandler(_learnerRepository.Object, _bursaryRepository.Object);
    }

    private static Learner BuildLearner(string province = "Gauteng") => Learner.Create(
        "user-1", "Thabo", "Nkosi", new DateOnly(2005, 1, 1),
        "0712345678", province, "School", province, GradeLevel.Grade12, true, "v1");

    private void SetupLearner(Learner learner, int? aps)
    {
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);

        Domain.Entities.AcademicProfile? profile = null;
        if (aps is not null)
        {
            profile = Domain.Entities.AcademicProfile.Create(learner.Id, 2025, ResultType.Grade12Final);
            profile.SetApsScore(aps.Value);
        }
        _learnerRepository.Setup(x => x.GetAcademicProfileByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
    }

    private void SetupBursaries(params Bursary[] bursaries)
        => _bursaryRepository.Setup(x => x.GetAllActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bursaries);

    [Fact]
    public async Task Handle_ApsAtOrAboveMinimum_Matches()
    {
        var learner = BuildLearner();
        SetupLearner(learner, aps: 40);
        SetupBursaries(Bursary.Create("B", "P", "d", BursaryFundingType.TuitionOnly, minimumAps: 40));

        var result = (await _sut.Handle(new GetBursaryMatchesQuery("user-1"), CancellationToken.None)).ToList();

        result.Should().HaveCount(1);
        result[0].GuidanceOnly.Should().BeTrue();
        result[0].Reasons.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ApsBelowMinimum_DoesNotMatch()
    {
        var learner = BuildLearner();
        SetupLearner(learner, aps: 30);
        SetupBursaries(Bursary.Create("B", "P", "d", BursaryFundingType.TuitionOnly, minimumAps: 40));

        var result = await _sut.Handle(new GetBursaryMatchesQuery("user-1"), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_NullMinimumAps_AlwaysMatches()
    {
        var learner = BuildLearner();
        SetupLearner(learner, aps: null);
        SetupBursaries(Bursary.Create("B", "P", "d", BursaryFundingType.Stipend, minimumAps: null));

        var result = await _sut.Handle(new GetBursaryMatchesQuery("user-1"), CancellationToken.None);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ProvinceFilter_Respected()
    {
        var learner = BuildLearner(province: "Limpopo");
        SetupLearner(learner, aps: 50);
        SetupBursaries(
            Bursary.Create("InProvince", "P", "d", BursaryFundingType.Stipend, minimumAps: null, provincesEligible: ["Limpopo"]),
            Bursary.Create("OutProvince", "P", "d", BursaryFundingType.Stipend, minimumAps: null, provincesEligible: ["Gauteng"]));

        var result = (await _sut.Handle(new GetBursaryMatchesQuery("user-1"), CancellationToken.None)).ToList();

        result.Should().ContainSingle(m => m.Name == "InProvince");
    }

    [Fact]
    public async Task Handle_LearnerNotFound_ThrowsKeyNotFound()
    {
        _learnerRepository.Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Learner?)null);

        var act = () => _sut.Handle(new GetBursaryMatchesQuery("user-1"), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
