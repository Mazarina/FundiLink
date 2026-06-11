import { useEffect, useState } from 'react'
import { getNotificationHistory } from '../features/notifications/notificationsApi'
import type { NotificationLogEntry } from '../types'

const TYPE_LABELS: Record<string, string> = {
  DeadlineReminder: 'Deadline reminder',
  ApplicationStatusChange: 'Application status update',
  DocumentVerificationResult: 'Document verification',
  RegistrationWelcome: 'Welcome',
  BursaryStatusChange: 'Bursary status update',
}

function statusClasses(status: string): string {
  switch (status) {
    case 'Sent':
      return 'bg-green-50 text-green-700 border-green-200'
    case 'Failed':
      return 'bg-red-50 text-red-700 border-red-200'
    default:
      return 'bg-gray-50 text-gray-600 border-gray-200'
  }
}

export default function NotificationHistoryPage() {
  const [items, setItems] = useState<NotificationLogEntry[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    getNotificationHistory()
      .then(setItems)
      .catch(() => setError('Could not load your notification history.'))
      .finally(() => setLoading(false))
  }, [])

  if (loading) return (
    <main className="min-h-screen bg-brand-light flex items-center justify-center p-4">
      <p className="text-gray-500">Loading your notifications...</p>
    </main>
  )

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto space-y-4">
        <h1 className="text-2xl font-bold text-brand-primary">Notification History</h1>
        <p className="text-sm text-gray-500">
          Reminders and updates FundiLink has generated for you. Reminders are guidance only —
          FundiLink is not an official admissions or funding portal.
        </p>

        {error && (
          <div className="p-3 bg-red-50 border border-red-200 rounded text-red-700 text-sm">{error}</div>
        )}

        {!error && items.length === 0 && (
          <div className="bg-white rounded-xl p-6 shadow-sm text-center text-gray-500">
            You have no notifications yet.
          </div>
        )}

        {items.length > 0 && (
          <ul className="space-y-3">
            {items.map((n) => (
              <li key={n.id} className="bg-white rounded-xl p-4 shadow-sm flex items-center justify-between">
                <div>
                  <p className="font-medium text-gray-800">{TYPE_LABELS[n.notificationType] ?? n.notificationType}</p>
                  <p className="text-xs text-gray-500">
                    {n.channel} · {new Date(n.sentAt).toLocaleString()}
                  </p>
                </div>
                <span className={`text-xs px-2 py-1 rounded border ${statusClasses(n.status)}`}>{n.status}</span>
              </li>
            ))}
          </ul>
        )}
      </div>
    </main>
  )
}
