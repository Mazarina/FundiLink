using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Admin.Commands.CreateBursary;
using FundiLink.Application.Features.Admin.Commands.UpdateBursary;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Admin;

public class CreateUpdateBursaryHandlerTests
{
    private readonly Mock<IBursaryRepository> _bursaryRepository = new();
    private readonly Mock<IAuditLogRepository> _auditLogRepository = new();

    [Fact]
    public async Task CreateBursary_WritesAuditLog()
    {
        var sut = new CreateBursaryHandler(_bursaryRepository.Object, _auditLogRepository.Object);

        var id = await sut.Handle(new CreateBursaryCommand(
            "actor", "Admin", "Name", "Provider", "Desc", BursaryFundingType.FullCost,
            [], null, null, [], null, null, null), CancellationToken.None);

        id.Should().NotBeEmpty();
        _bursaryRepository.Verify(x => x.AddAsync(It.IsAny<Bursary>(), It.IsAny<CancellationToken>()), Times.Once);
        _auditLogRepository.Verify(x => x.AddAsync(
            It.Is<AuditLogEntry>(e => e.Action == "CreateBursary" && e.TargetType == "Bursary"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBursary_WritesAuditLog()
    {
        var bursary = Bursary.Create("Name", "Provider", "Desc", BursaryFundingType.FullCost);
        _bursaryRepository.Setup(x => x.GetByIdAsync(bursary.Id, It.IsAny<CancellationToken>())).ReturnsAsync(bursary);
        var sut = new UpdateBursaryHandler(_bursaryRepository.Object, _auditLogRepository.Object);

        await sut.Handle(new UpdateBursaryCommand(
            "actor", "Admin", bursary.Id, "New", "Provider", "Desc", BursaryFundingType.Stipend,
            [], null, null, [], null, null, null, true), CancellationToken.None);

        bursary.Name.Should().Be("New");
        _auditLogRepository.Verify(x => x.AddAsync(
            It.Is<AuditLogEntry>(e => e.Action == "UpdateBursary" && e.TargetId == bursary.Id.ToString()),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
