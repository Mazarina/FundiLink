import { useEffect, useState } from 'react'
import { getNotificationPreferences, updateNotificationPreferences } from '../features/notifications/notificationsApi'
import type { NotificationPreferences } from '../types'

export default function NotificationPreferencesPage() {
  const [prefs, setPrefs] = useState<NotificationPreferences>({
    emailEnabled: true,
    whatsAppEnabled: false,
    smsEnabled: false,
  })
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')

  useEffect(() => {
    getNotificationPreferences()
      .then(setPrefs)
      .catch(() => setError('Could not load your notification preferences.'))
      .finally(() => setLoading(false))
  }, [])

  function toggle(field: keyof NotificationPreferences) {
    setPrefs((p) => ({ ...p, [field]: !p[field] }))
    setSuccess('')
  }

  async function handleSave() {
    setError('')
    setSuccess('')
    setSaving(true)
    try {
      await updateNotificationPreferences(prefs)
      setSuccess('Your notification preferences have been saved.')
    } catch {
      setError('Failed to save. Please try again.')
    } finally {
      setSaving(false)
    }
  }

  if (loading) return (
    <main className="min-h-screen bg-brand-light flex items-center justify-center p-4">
      <p className="text-gray-500">Loading your preferences...</p>
    </main>
  )

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto space-y-4">
        <h1 className="text-2xl font-bold text-brand-primary">Notification Settings</h1>

        <div className="bg-white rounded-xl p-6 shadow-sm">
          <h2 className="font-semibold text-gray-800 mb-4">How would you like to hear from us?</h2>

          {error && <div className="mb-3 p-3 bg-red-50 border border-red-200 rounded text-red-700 text-sm">{error}</div>}
          {success && <div className="mb-3 p-3 bg-green-50 border border-green-200 rounded text-green-700 text-sm">{success}</div>}

          <div className="space-y-4">
            <label className="flex items-center justify-between">
              <span className="text-sm font-medium text-gray-700">Email notifications</span>
              <input
                type="checkbox"
                aria-label="Email notifications"
                checked={prefs.emailEnabled}
                onChange={() => toggle('emailEnabled')}
                className="h-5 w-5"
              />
            </label>

            <label className="flex items-center justify-between">
              <span className="text-sm font-medium text-gray-700">WhatsApp notifications</span>
              <input
                type="checkbox"
                aria-label="WhatsApp notifications"
                checked={prefs.whatsAppEnabled}
                onChange={() => toggle('whatsAppEnabled')}
                className="h-5 w-5"
              />
            </label>

            <label className="flex items-center justify-between">
              <span className="text-sm font-medium text-gray-700">SMS notifications</span>
              <input
                type="checkbox"
                aria-label="SMS notifications"
                checked={prefs.smsEnabled}
                onChange={() => toggle('smsEnabled')}
                className="h-5 w-5"
              />
            </label>
          </div>

          <p className="text-xs text-gray-400 mt-4">
            WhatsApp and SMS notifications are coming soon — your preference will be saved and used once available.
          </p>

          <button
            onClick={handleSave}
            disabled={saving}
            className="mt-5 w-full bg-brand-primary text-white px-6 py-2 rounded-lg text-sm font-semibold hover:opacity-90 disabled:opacity-50"
          >
            {saving ? 'Saving...' : 'Save preferences'}
          </button>
        </div>
      </div>
    </main>
  )
}
