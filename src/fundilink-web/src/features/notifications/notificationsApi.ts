import client from '../../api/client'
import type {
  NotificationLogEntry,
  NotificationPreferences,
  ReminderRunResult,
} from '../../types'

export async function getNotificationPreferences(): Promise<NotificationPreferences> {
  const res = await client.get('/notifications/preferences')
  return res.data
}

export async function updateNotificationPreferences(prefs: NotificationPreferences): Promise<void> {
  await client.put('/notifications/preferences', prefs)
}

// Owner-scoped notification history (Phase 12). Read-only view of the append-only log.
export async function getNotificationHistory(): Promise<NotificationLogEntry[]> {
  const res = await client.get('/notifications/history')
  return res.data
}

// Admin/ops-triggered deadline-reminder pass (Phase 12). RBAC-gated server-side and
// audit-logged. Stub delivery providers only — reminders are guidance.
export async function runDeadlineReminders(windowDays: number): Promise<ReminderRunResult> {
  const res = await client.post('/notifications/admin/run-deadline-reminders', { windowDays })
  return res.data
}
