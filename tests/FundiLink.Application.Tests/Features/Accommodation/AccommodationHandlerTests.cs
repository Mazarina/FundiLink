using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Accommodation.Commands.TrackAccommodationInterest;
using FundiLink.Application.Features.Accommodation.Commands.UpdateAccommodationInterestStatus;
using FundiLink.Application.Features.Accommodation.Queries.GetAccommodationMatches;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Accommodation;

public class AccommodationHandlerTests
{
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<IAccommodationRepository> _accommodationRepository = new();
    private readonly Mock<IAccommodationInterestRepository> _interestRepository = new();

    private static Learner BuildLearner(string province = "Gauteng") => Learner.Create(
        "user-1", "Thabo", "Nkosi", new DateOnly(2005, 1, 1),
        "0712345678", province, "School", province, GradeLevel.Grade12, true, "v1");

    private static AccommodationListing BuildListing(string province = "Gauteng") =>
        AccommodationListing.Create("Res", "Provider", "Desc", AccommodationType.Room, province, "City");

    [Fact]
    public async Task Matches_ListingInLearnerProvince_MatchesWithReason()
    {
        var learner = BuildLearner("Limpopo");
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _accommodationRepository.Setup(x => x.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { BuildListing("Limpopo"), BuildListing("Gauteng") });
        var sut = new GetAccommodationMatchesHandler(_learnerRepository.Object, _accommodationRepository.Object);

        var result = (await sut.Handle(new GetAccommodationMatchesQuery("user-1"), CancellationToken.None)).ToList();

        result.Should().HaveCount(1);
        result[0].Province.Should().Be("Limpopo");
        result[0].GuidanceOnly.Should().BeTrue();
        result[0].Reasons.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Matches_LearnerNotFound_ThrowsKeyNotFound()
    {
        _learnerRepository.Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Learner?)null);
        var sut = new GetAccommodationMatchesHandler(_learnerRepository.Object, _accommodationRepository.Object);

        var act = () => sut.Handle(new GetAccommodationMatchesQuery("user-1"), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task TrackInterest_New_CreatesAndReturnsId()
    {
        var learner = BuildLearner();
        var listing = BuildListing();
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _accommodationRepository.Setup(x => x.GetByIdAsync(listing.Id, It.IsAny<CancellationToken>())).ReturnsAsync(listing);
        _interestRepository.Setup(x => x.GetByLearnerAndListingAsync(learner.Id, listing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AccommodationInterest?)null);
        var sut = new TrackAccommodationInterestHandler(_learnerRepository.Object, _accommodationRepository.Object, _interestRepository.Object);

        var result = await sut.Handle(new TrackAccommodationInterestCommand(listing.Id, "user-1"), CancellationToken.None);

        result.Should().NotBeEmpty();
        _interestRepository.Verify(x => x.AddAsync(It.Is<AccommodationInterest>(i =>
            i.LearnerId == learner.Id && i.AccommodationListingId == listing.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TrackInterest_Existing_UpdatesInsteadOfDuplicating()
    {
        var learner = BuildLearner();
        var listing = BuildListing();
        var existing = AccommodationInterest.Create(learner.Id, listing.Id);
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _accommodationRepository.Setup(x => x.GetByIdAsync(listing.Id, It.IsAny<CancellationToken>())).ReturnsAsync(listing);
        _interestRepository.Setup(x => x.GetByLearnerAndListingAsync(learner.Id, listing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        var sut = new TrackAccommodationInterestHandler(_learnerRepository.Object, _accommodationRepository.Object, _interestRepository.Object);

        var result = await sut.Handle(
            new TrackAccommodationInterestCommand(listing.Id, "user-1", OpportunityInterestStatus.Contacted), CancellationToken.None);

        result.Should().Be(existing.Id);
        existing.Status.Should().Be(OpportunityInterestStatus.Contacted);
        _interestRepository.Verify(x => x.AddAsync(It.IsAny<AccommodationInterest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStatus_UnauthorisedLearner_Rejected()
    {
        var learner = BuildLearner();
        var interest = AccommodationInterest.Create(Guid.NewGuid(), Guid.NewGuid()); // owned by someone else
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _interestRepository.Setup(x => x.GetByIdAsync(interest.Id, It.IsAny<CancellationToken>())).ReturnsAsync(interest);
        var sut = new UpdateAccommodationInterestStatusHandler(_learnerRepository.Object, _interestRepository.Object);

        var act = () => sut.Handle(
            new UpdateAccommodationInterestStatusCommand(interest.Id, "user-1", OpportunityInterestStatus.Applied, null), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        _interestRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
