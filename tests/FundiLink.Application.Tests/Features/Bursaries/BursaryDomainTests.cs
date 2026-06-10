using FluentAssertions;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;

namespace FundiLink.Application.Tests.Features.Bursaries;

public class BursaryDomainTests
{
    [Fact]
    public void Create_SetsFieldsAndSerialisesCollections()
    {
        var bursary = Bursary.Create(
            "Name", "Provider", "Description", BursaryFundingType.FullCost,
            fieldsOfStudy: ["Engineering", "Science"],
            minimumAps: 38,
            provincesEligible: ["Gauteng"]);

        bursary.Name.Should().Be("Name");
        bursary.FundingType.Should().Be(BursaryFundingType.FullCost);
        bursary.FieldsOfStudy.Should().BeEquivalentTo("Engineering", "Science");
        bursary.ProvincesEligible.Should().ContainSingle().Which.Should().Be("Gauteng");
        bursary.MinimumAps.Should().Be(38);
        bursary.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Update_ReplacesValues()
    {
        var bursary = Bursary.Create("Name", "Provider", "Description", BursaryFundingType.FullCost);

        bursary.Update("New", "NewProvider", "NewDesc", BursaryFundingType.Stipend,
            fieldsOfStudy: ["Law"], minimumAps: 30, maxHouseholdIncome: 350000m,
            provincesEligible: [], applicationOpenDate: null, applicationCloseDate: null,
            externalApplicationUrl: "https://example.org", isActive: false);

        bursary.Name.Should().Be("New");
        bursary.FundingType.Should().Be(BursaryFundingType.Stipend);
        bursary.FieldsOfStudy.Should().ContainSingle().Which.Should().Be("Law");
        bursary.MaxHouseholdIncome.Should().Be(350000m);
        bursary.IsActive.Should().BeFalse();
    }

    [Fact]
    public void BursaryApplication_UpdateStatus_ChangesStatusAndNotes()
    {
        var app = BursaryApplication.Create(Guid.NewGuid(), Guid.NewGuid());

        app.UpdateStatus(BursaryApplicationStatus.Awarded, "Got it");

        app.Status.Should().Be(BursaryApplicationStatus.Awarded);
        app.Notes.Should().Be("Got it");
    }
}
