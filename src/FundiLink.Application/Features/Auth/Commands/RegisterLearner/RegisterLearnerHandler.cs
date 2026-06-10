using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Auth.Commands.RegisterLearner;

public class RegisterLearnerHandler : IRequestHandler<RegisterLearnerCommand, RegisterLearnerResult>
{
    private readonly IIdentityService _identityService;
    private readonly ILearnerRepository _learnerRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;

    public RegisterLearnerHandler(
        IIdentityService identityService,
        ILearnerRepository learnerRepository,
        IApplicationDbContext dbContext,
        IEmailService emailService,
        INotificationService notificationService)
    {
        _identityService = identityService;
        _learnerRepository = learnerRepository;
        _dbContext = dbContext;
        _emailService = emailService;
        _notificationService = notificationService;
    }

    public async Task<RegisterLearnerResult> Handle(RegisterLearnerCommand request, CancellationToken cancellationToken)
    {
        if (!request.ConsentAccepted)
            throw new InvalidOperationException("POPIA consent is required.");

        var (succeeded, userId, errors) = await _identityService.CreateUserAsync(request.Email, request.Password);
        if (!succeeded)
            throw new InvalidOperationException($"Registration failed: {string.Join("; ", errors)}");

        await _identityService.AssignRoleAsync(userId, "Student");

        var learner = Learner.Create(
            userId,
            request.FirstName,
            request.Surname,
            request.DateOfBirth,
            request.MobileNumber,
            request.Province,
            request.SchoolName,
            request.SchoolProvince,
            request.GradeLevel,
            request.ConsentAccepted,
            consentVersion: "1.0"
        );

        await _learnerRepository.AddAsync(learner, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var token = await _identityService.GenerateEmailConfirmationTokenAsync(userId);
        await _emailService.SendEmailVerificationAsync(
            request.Email,
            $"/verify?token={Uri.EscapeDataString(token)}&userId={userId}",
            cancellationToken);

        await _notificationService.NotifyAsync(
            learner.Id,
            NotificationType.RegistrationWelcome,
            "Welcome to FundiLink",
            "Welcome to FundiLink! Your profile is ready. Complete it to discover programmes and opportunities you qualify for.",
            cancellationToken);

        return new RegisterLearnerResult(userId, "Registration successful. Please verify your email.");
    }
}
