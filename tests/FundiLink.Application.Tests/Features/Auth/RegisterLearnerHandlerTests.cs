using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Auth.Commands.RegisterLearner;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Auth;

public class RegisterLearnerHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<IApplicationDbContext> _dbContext = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly RegisterLearnerHandler _sut;

    public RegisterLearnerHandlerTests()
    {
        _sut = new RegisterLearnerHandler(
            _identityService.Object,
            _learnerRepository.Object,
            _dbContext.Object,
            _emailService.Object);
    }

    private static RegisterLearnerCommand ValidCommand(bool consent = true) => new(
        Email: "thabo@example.com",
        Password: "SecurePass123!",
        FirstName: "Thabo",
        Surname: "Nkosi",
        DateOfBirth: new DateOnly(2000, 5, 15),
        MobileNumber: "0712345678",
        Province: "Limpopo",
        SchoolName: "Sekhukhune High",
        SchoolProvince: "Limpopo",
        GradeLevel: GradeLevel.Grade12,
        ConsentAccepted: consent
    );

    [Fact]
    public async Task Handle_ValidCommand_CreatesUserAndLearner()
    {
        _identityService.Setup(x => x.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((true, "user-123", Array.Empty<string>()));
        _identityService.Setup(x => x.AssignRoleAsync("user-123", "Student")).ReturnsAsync(true);
        _identityService.Setup(x => x.GenerateEmailConfirmationTokenAsync("user-123")).ReturnsAsync("token");
        _learnerRepository.Setup(x => x.AddAsync(It.IsAny<Learner>(), default)).Returns(Task.CompletedTask);
        _dbContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
        _emailService.Setup(x => x.SendEmailVerificationAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .Returns(Task.CompletedTask);

        var result = await _sut.Handle(ValidCommand(), CancellationToken.None);

        result.UserId.Should().Be("user-123");
        result.Message.Should().Contain("Registration successful");
        _learnerRepository.Verify(x => x.AddAsync(It.IsAny<Learner>(), default), Times.Once);
        _dbContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_ConsentNotAccepted_ThrowsInvalidOperationException()
    {
        var act = () => _sut.Handle(ValidCommand(consent: false), CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*consent*");
    }

    [Fact]
    public async Task Handle_IdentityCreateFails_ThrowsInvalidOperationException()
    {
        _identityService.Setup(x => x.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((false, "", new[] { "Password too weak." }));

        var act = () => _sut.Handle(ValidCommand(), CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Registration failed*");
    }

    [Fact]
    public async Task Handle_ValidCommand_SendsVerificationEmail()
    {
        _identityService.Setup(x => x.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((true, "user-456", Array.Empty<string>()));
        _identityService.Setup(x => x.AssignRoleAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _identityService.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<string>())).ReturnsAsync("confirm-token");
        _learnerRepository.Setup(x => x.AddAsync(It.IsAny<Learner>(), default)).Returns(Task.CompletedTask);
        _dbContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
        _emailService.Setup(x => x.SendEmailVerificationAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .Returns(Task.CompletedTask);

        await _sut.Handle(ValidCommand(), CancellationToken.None);

        _emailService.Verify(x => x.SendEmailVerificationAsync(
            "thabo@example.com", It.IsAny<string>(), default), Times.Once);
    }
}
