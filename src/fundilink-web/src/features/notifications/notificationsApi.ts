import client from '../../api/client'
import type { NotificationPreferences } from '../../types'

export async function getNotificationPreferences(): Promise<NotificationPreferences> {
  const res = await client.get('/notifications/preferences')
  return res.data
}

export async function updateNotificationPreferences(prefs: NotificationPreferences): Promise<void> {
  await client.put('/notifications/preferences', prefs)
}
