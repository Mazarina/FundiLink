using FundiLink.Domain.Common;

namespace FundiLink.Domain.Entities;

public class NotificationPreference : BaseEntity
{
    public Guid LearnerId { get; private set; }
    public bool EmailEnabled { get; private set; } = true;
    public bool WhatsAppEnabled { get; private set; }
    public bool SmsEnabled { get; private set; }

    private NotificationPreference() { }

    public static NotificationPreference CreateDefault(Guid learnerId)
    {
        return new NotificationPreference
        {
            LearnerId = learnerId,
            EmailEnabled = true,
            WhatsAppEnabled = false,
            SmsEnabled = false
        };
    }

    public void UpdatePreferences(bool email, bool whatsApp, bool sms)
    {
        EmailEnabled = email;
        WhatsAppEnabled = whatsApp;
        SmsEnabled = sms;
        MarkUpdated();
    }
}
