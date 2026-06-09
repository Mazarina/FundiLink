# Phase 5 — WhatsApp / SMS / Multi-Channel Notifications

You are continuing FundiLink by ZulTek. Work in the existing repo. Branch: `claude/happy-dirac-n7qgtg`.

**dotnet is at `/usr/local/dotnet/dotnet`**

Phase 4 (document vault + admin portal) is complete. Build Phase 5: a multi-channel
notification system. No real third-party integrations yet — stub external channels behind
interfaces (no Meta Business / SMS gateway credentials exist or should be committed).

## Scope

### Domain
- `NotificationChannel` enum: Email, WhatsApp, Sms
- `NotificationType` enum: DeadlineReminder, ApplicationStatusChange, DocumentVerificationResult, RegistrationWelcome
- `NotificationPreference` entity: per-learner opt in/out per channel (defaults: Email on, others off)
- `NotificationLog` entity: append-only record of every notification attempt (learnerId, type,
  channel, recipient, status, sentAt, errorMessage). No update/delete.

### Application
- `IWhatsAppService`, `ISmsService` abstractions
- `INotificationService` abstraction that, given a learner + notification type + payload, resolves
  the learner's preferences and dispatches to each opted-in channel, writing a NotificationLog per attempt
- CQRS handlers: `UpdateNotificationPreferences`, `GetNotificationPreferences`, `SendNotification` (internal use)
- Repositories: `INotificationPreferenceRepository`, `INotificationLogRepository` (append-only)

### Infrastructure
- `StubWhatsAppService`, `StubSmsService` (log only, always "sent")
- `EmailNotificationService` wrapping the existing `IEmailService` stub
- `NotificationService` implementing `INotificationService` with channel selection by preference
- EF configs + DbSets + migration `AddNotifications`
- Register all services in DI
- Scheduler placeholder only — service layer, no real cron

### API
- `GET /api/v1/notifications/preferences`
- `PUT /api/v1/notifications/preferences`
- Wire status-change / document-verification / welcome events to call INotificationService

### Frontend (`src/fundilink-web`)
- `NotificationPreferencesPage` at `/notifications/preferences` — toggle per channel
- Settings tile on ProfilePage linking to it
- (Notification bell icon in header — placeholder, future)

### Tests
- Notification service unit tests: correct channels selected per preference combination
- NotificationLog append-only behaviour
- Preference update/get handler tests
- Frontend: preferences page renders and toggles

## Definition of done
- `/usr/local/dotnet/dotnet build FundiLink.sln` clean
- Migration generated
- `/usr/local/dotnet/dotnet test` all green
- `cd src/fundilink-web && npm run build && npm test -- --run` clean
- Update ROADMAP.md Phase 5 to Complete; write Phase 6 prompt here
- Commit + push to `claude/happy-dirac-n7qgtg`

**Never commit secrets. Stub all external channels.**
