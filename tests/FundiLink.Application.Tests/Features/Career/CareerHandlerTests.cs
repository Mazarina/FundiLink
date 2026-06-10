using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Career.Commands.TrackCareerInterest;
using FundiLink.Application.Features.Career.Commands.UpdateCareerInterestStatus;
using FundiLink.Application.Features.Career.Queries.GetCareerMatches;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Career;

public class CareerHandlerTests
{
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<ICareerRepository> _careerRepository = new();
    private readonly Mock<ICareerInterestRepository> _interestRepository = new();

    private static Learner BuildLearner(GradeLevel grade = GradeLevel.Grade12, string province = "Gauteng") => Learner.Create(
        "user-1", "Thabo", "Nkosi", new DateOnly(2005, 1, 1),
        "0712345678", province, "School", province, grade, true, "v1");

    private void SetupOpportunities(params CareerOpportunity[] opportunities)
        => _careerRepository.Setup(x => x.GetAllActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(opportunities);

    [Fact]
    public async Task Matches_GradeAtOrAboveMinimum_Matches()
    {
        var learner = BuildLearner(GradeLevel.PostMatric);
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        SetupOpportunities(CareerOpportunity.Create("Internship", "P", "d", CareerOpportunityType.Internship, minimumGradeLevel: GradeLevel.Grade12));
        var sut = new GetCareerMatchesHandler(_learnerRepository.Object, _careerRepository.Object);

        var result = (await sut.Handle(new GetCareerMatchesQuery("user-1"), CancellationToken.None)).ToList();

        result.Should().HaveCount(1);
        result[0].GuidanceOnly.Should().BeTrue();
        result[0].Reasons.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Matches_GradeBelowMinimum_DoesNotMatch()
    {
        var learner = BuildLearner(GradeLevel.Grade11);
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        SetupOpportunities(CareerOpportunity.Create("Learnership", "P", "d", CareerOpportunityType.Learnership, minimumGradeLevel: GradeLevel.Grade12));
        var sut = new GetCareerMatchesHandler(_learnerRepository.Object, _careerRepository.Object);

        var result = await sut.Handle(new GetCareerMatchesQuery("user-1"), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Matches_ProvinceFilter_Respected()
    {
        var learner = BuildLearner(GradeLevel.Grade12, "Limpopo");
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        SetupOpportunities(
            CareerOpportunity.Create("InProvince", "P", "d", CareerOpportunityType.SkillsProgramme, provincesEligible: ["Limpopo"]),
            CareerOpportunity.Create("OutProvince", "P", "d", CareerOpportunityType.SkillsProgramme, provincesEligible: ["Gauteng"]));
        var sut = new GetCareerMatchesHandler(_learnerRepository.Object, _careerRepository.Object);

        var result = (await sut.Handle(new GetCareerMatchesQuery("user-1"), CancellationToken.None)).ToList();

        result.Should().ContainSingle(m => m.Title == "InProvince");
    }

    [Fact]
    public async Task TrackInterest_New_CreatesAndReturnsId()
    {
        var learner = BuildLearner();
        var opportunity = CareerOpportunity.Create("T", "P", "d", CareerOpportunityType.Internship);
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _careerRepository.Setup(x => x.GetByIdAsync(opportunity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(opportunity);
        _interestRepository.Setup(x => x.GetByLearnerAndOpportunityAsync(learner.Id, opportunity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CareerInterest?)null);
        var sut = new TrackCareerInterestHandler(_learnerRepository.Object, _careerRepository.Object, _interestRepository.Object);

        var result = await sut.Handle(new TrackCareerInterestCommand(opportunity.Id, "user-1"), CancellationToken.None);

        result.Should().NotBeEmpty();
        _interestRepository.Verify(x => x.AddAsync(It.Is<CareerInterest>(i =>
            i.LearnerId == learner.Id && i.CareerOpportunityId == opportunity.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_OwnerScoped_Updates()
    {
        var learner = BuildLearner();
        var interest = CareerInterest.Create(learner.Id, Guid.NewGuid());
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _interestRepository.Setup(x => x.GetByIdAsync(interest.Id, It.IsAny<CancellationToken>())).ReturnsAsync(interest);
        var sut = new UpdateCareerInterestStatusHandler(_learnerRepository.Object, _interestRepository.Object);

        await sut.Handle(new UpdateCareerInterestStatusCommand(interest.Id, "user-1", OpportunityInterestStatus.Applied, null), CancellationToken.None);

        interest.Status.Should().Be(OpportunityInterestStatus.Applied);
        _interestRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_UnauthorisedLearner_Rejected()
    {
        var learner = BuildLearner();
        var interest = CareerInterest.Create(Guid.NewGuid(), Guid.NewGuid());
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _interestRepository.Setup(x => x.GetByIdAsync(interest.Id, It.IsAny<CancellationToken>())).ReturnsAsync(interest);
        var sut = new UpdateCareerInterestStatusHandler(_learnerRepository.Object, _interestRepository.Object);

        var act = () => sut.Handle(
            new UpdateCareerInterestStatusCommand(interest.Id, "user-1", OpportunityInterestStatus.Applied, null), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
