import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import {
  getBursaryApplications,
  updateBursaryApplicationStatus,
  deleteBursaryApplication,
} from '../features/bursaries/bursariesApi'
import { BursaryDisclaimerBanner } from '../features/bursaries/BursaryDisclaimerBanner'
import type { BursaryApplication, BursaryApplicationStatus } from '../types'

const STATUSES: BursaryApplicationStatus[] = ['Researching', 'Preparing', 'Submitted', 'Awarded', 'Rejected']

export default function BursaryApplicationsPage() {
  const [applications, setApplications] = useState<BursaryApplication[] | null>(null)
  const [error, setError] = useState('')

  function load() {
    getBursaryApplications()
      .then(setApplications)
      .catch(() => setError('Could not load your bursary applications.'))
  }

  useEffect(() => {
    load()
  }, [])

  async function handleStatusChange(id: string, newStatus: BursaryApplicationStatus) {
    setError('')
    try {
      await updateBursaryApplicationStatus(id, { newStatus })
      load()
    } catch {
      setError('Could not update status. Please try again.')
    }
  }

  async function handleDelete(id: string) {
    setError('')
    try {
      await deleteBursaryApplication(id)
      load()
    } catch {
      setError('Could not remove this bursary application.')
    }
  }

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto space-y-4">
        <header className="flex justify-between items-center">
          <h1 className="text-2xl font-bold text-brand-primary">My Bursary Applications</h1>
          <Link to="/bursaries" className="text-sm text-brand-primary hover:underline">Browse bursaries</Link>
        </header>

        <BursaryDisclaimerBanner />

        {error && <p className="text-red-600 text-sm">{error}</p>}

        {applications === null ? (
          <p className="text-gray-500 text-center py-8">Loading...</p>
        ) : applications.length === 0 ? (
          <div className="bg-white rounded-xl p-6 shadow-sm text-center text-gray-600">
            No bursaries tracked yet. Browse bursaries to get started.
          </div>
        ) : (
          <div className="space-y-3">
            {applications.map((a) => (
              <div key={a.id} className="bg-white rounded-xl p-5 shadow-sm space-y-3">
                <div className="flex justify-between items-start gap-3">
                  <div className="min-w-0">
                    <p className="text-xs text-gray-500 truncate">{a.providerName}</p>
                    <h3 className="font-semibold text-gray-800">{a.bursaryName}</h3>
                  </div>
                  <button onClick={() => handleDelete(a.id)} className="text-xs text-gray-400 hover:text-red-500">
                    Remove
                  </button>
                </div>
                <div className="flex items-center gap-2">
                  <label htmlFor={`status-${a.id}`} className="text-xs text-gray-500">Status</label>
                  <select
                    id={`status-${a.id}`}
                    aria-label={`Status for ${a.bursaryName}`}
                    value={a.status}
                    onChange={(e) => handleStatusChange(a.id, e.target.value as BursaryApplicationStatus)}
                    className="border rounded-lg px-2 py-1 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-brand-primary"
                  >
                    {STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
                  </select>
                </div>
                {a.externalApplicationUrl && (
                  <a href={a.externalApplicationUrl} target="_blank" rel="noopener noreferrer"
                    className="text-xs text-brand-primary hover:underline">
                    Funder's official portal →
                  </a>
                )}
              </div>
            ))}
          </div>
        )}
      </div>
    </main>
  )
}
