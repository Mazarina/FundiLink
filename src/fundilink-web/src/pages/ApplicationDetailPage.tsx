import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import {
  deleteApplication,
  getApplication,
  updateApplicationStatus,
} from '../features/applications/applicationsApi'
import { ApplicationStatusBadge } from '../features/applications/ApplicationStatusBadge'
import type { ApplicationStatus, LearnerApplication } from '../types'

const STATUSES: ApplicationStatus[] = [
  'Interested', 'InProgress', 'Submitted', 'Accepted', 'Rejected', 'Waitlisted',
]

export default function ApplicationDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [application, setApplication] = useState<LearnerApplication | null>(null)
  const [status, setStatus] = useState<ApplicationStatus>('Interested')
  const [notes, setNotes] = useState('')
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    if (!id) return
    getApplication(id)
      .then((a) => {
        setApplication(a)
        setStatus(a.status)
        setNotes(a.notes ?? '')
      })
      .catch(() => setError('Could not load this application.'))
  }, [id])

  async function handleSave() {
    if (!id) return
    setSaving(true)
    setError('')
    setSuccess('')
    try {
      await updateApplicationStatus(id, { newStatus: status, notes })
      setSuccess('Application updated.')
    } catch {
      setError('Could not update. Please try again.')
    } finally {
      setSaving(false)
    }
  }

  async function handleDelete() {
    if (!id) return
    if (!window.confirm('Remove this tracked application? This cannot be undone.')) return
    try {
      await deleteApplication(id)
      navigate('/applications')
    } catch {
      setError('Could not delete. Please try again.')
    }
  }

  if (!application) return (
    <main className="min-h-screen bg-brand-light flex items-center justify-center p-4">
      <p className="text-gray-500">{error || 'Loading application...'}</p>
    </main>
  )

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto space-y-4">
        <Link to="/applications" className="text-sm text-gray-500 hover:underline">← Back to applications</Link>

        <div className="bg-white rounded-xl p-6 shadow-sm space-y-2">
          <p className="text-sm text-gray-500">{application.institutionName}</p>
          <div className="flex justify-between items-center">
            <h1 className="text-2xl font-bold text-brand-primary">{application.programmeName}</h1>
            <ApplicationStatusBadge status={application.status} />
          </div>
          <Link to={`/programmes/${application.programmeId}`} className="text-sm text-brand-primary hover:underline">
            View programme details →
          </Link>
        </div>

        <div className="bg-white rounded-xl p-6 shadow-sm space-y-4">
          {error && <div className="p-3 bg-red-50 border border-red-200 rounded text-red-700 text-sm">{error}</div>}
          {success && <div className="p-3 bg-green-50 border border-green-200 rounded text-green-700 text-sm">{success}</div>}

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Status</label>
            <select value={status} onChange={(e) => setStatus(e.target.value as ApplicationStatus)}
              className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary">
              {STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Notes</label>
            <textarea value={notes} onChange={(e) => setNotes(e.target.value)} rows={4}
              className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary" />
          </div>

          <div className="flex gap-3">
            <button onClick={handleSave} disabled={saving}
              className="bg-brand-primary text-white px-6 py-2 rounded-lg text-sm font-semibold hover:opacity-90 disabled:opacity-50">
              {saving ? 'Saving...' : 'Save changes'}
            </button>
            <button onClick={handleDelete}
              className="ml-auto text-sm text-red-500 hover:text-red-700">Delete</button>
          </div>
        </div>
      </div>
    </main>
  )
}
