using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FundiLink.Infrastructure.Services;

/// <summary>
/// Multi-channel notification dispatcher. Honours each learner's opt-in
/// preferences (Email on by default, WhatsApp/SMS opt-in) and writes an
/// append-only NotificationLog entry for every channel attempted.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly INotificationPreferenceRepository _preferenceRepository;
    private readonly INotificationLogRepository _logRepository;
    private readonly IEmailService _emailService;
    private readonly IWhatsAppService _whatsAppService;
    private readonly ISmsService _smsService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        ILearnerRepository learnerRepository,
        INotificationPreferenceRepository preferenceRepository,
        INotificationLogRepository logRepository,
        IEmailService emailService,
        IWhatsAppService whatsAppService,
        ISmsService smsService,
        IIdentityService identityService,
        ILogger<NotificationService> logger)
    {
        _learnerRepository = learnerRepository;
        _preferenceRepository = preferenceRepository;
        _logRepository = logRepository;
        _emailService = emailService;
        _whatsAppService = whatsAppService;
        _smsService = smsService;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task NotifyAsync(Guid learnerId, NotificationType type, string subject, string message, CancellationToken ct)
    {
        var learner = await _learnerRepository.GetByIdAsync(learnerId, ct);
        if (learner is null)
        {
            _logger.LogWarning("NotifyAsync called for unknown learner {LearnerId}", learnerId);
            return;
        }

        var preference = await _preferenceRepository.GetByLearnerIdAsync(learnerId, ct);
        var emailEnabled = preference?.EmailEnabled ?? true;
        var whatsAppEnabled = preference?.WhatsAppEnabled ?? false;
        var smsEnabled = preference?.SmsEnabled ?? false;

        var logs = new List<NotificationLog>();

        if (emailEnabled)
        {
            var user = await _identityService.GetUserByIdAsync(learner.UserId);
            var email = user?.Email ?? string.Empty;
            logs.Add(await AttemptAsync(
                learnerId, type, NotificationChannel.Email, email,
                async () =>
                {
                    await _emailService.SendNotificationEmailAsync(email, subject, message, ct);
                    return true;
                }));
        }

        if (whatsAppEnabled)
        {
            logs.Add(await AttemptAsync(
                learnerId, type, NotificationChannel.WhatsApp, learner.MobileNumber,
                () => _whatsAppService.SendMessageAsync(learner.MobileNumber, message, ct)));
        }

        if (smsEnabled)
        {
            logs.Add(await AttemptAsync(
                learnerId, type, NotificationChannel.Sms, learner.MobileNumber,
                () => _smsService.SendSmsAsync(learner.MobileNumber, message, ct)));
        }

        foreach (var log in logs)
            await _logRepository.AddAsync(log, ct);

        await _logRepository.SaveChangesAsync(ct);
    }

    private async Task<NotificationLog> AttemptAsync(
        Guid learnerId,
        NotificationType type,
        NotificationChannel channel,
        string recipient,
        Func<Task<bool>> send)
    {
        try
        {
            var ok = await send();
            return ok
                ? NotificationLog.Create(learnerId, type, channel, recipient, NotificationStatus.Sent)
                : NotificationLog.Create(learnerId, type, channel, recipient, NotificationStatus.Failed, "Provider returned failure.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send {Channel} notification to learner {LearnerId}", channel, learnerId);
            return NotificationLog.Create(learnerId, type, channel, recipient, NotificationStatus.Failed, ex.Message);
        }
    }
}
