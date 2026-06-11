import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import {
  exportMyData,
  getMyErasureRequests,
  requestErasure,
} from '../features/data-rights/dataRightsApi'
import type { ErasureRequest } from '../types'

export default function DataRightsPage() {
  const [requests, setRequests] = useState<ErasureRequest[] | null>(null)
  const [error, setError] = useState('')
  const [reason, setReason] = useState('')
  const [busy, setBusy] = useState(false)

  function load() {
    getMyErasureRequests()
      .then(setRequests)
      .catch(() => setError('Could not load your data rights. Please try again.'))
  }

  useEffect(load, [])

  async function handleExport() {
    setBusy(true)
    try {
      const data = await exportMyData()
      const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' })
      const url = URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = 'fundilink-data-export.json'
      a.click()
      URL.revokeObjectURL(url)
    } finally {
      setBusy(false)
    }
  }

  async function handleRequestErasure() {
    setBusy(true)
    try {
      await requestErasure(reason.trim() || null)
      setReason('')
      load()
    } finally {
      setBusy(false)
    }
  }

  const hasOpenRequest = requests?.some(
    (r) => r.status === 'Requested' || r.status === 'Approved'
  )

  if (error) return <Centered text={error} className="text-red-600" />
  if (!requests) return <Centered text="Loading your data & privacy settings..." className="text-gray-500" />

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto">
        <header className="mb-6">
          <Link to="/profile" className="text-sm text-brand-primary hover:underline">&larr; Back to profile</Link>
          <h1 className="text-2xl font-bold text-brand-primary mt-2">My data &amp; privacy</h1>
        </header>

        <p className="text-sm text-gray-600 bg-white rounded-xl p-4 mb-4 shadow-sm">
          Under POPIA you can download a copy of your FundiLink data and request erasure of your
          personal information. An erasure request is reviewed and fulfilled by our team. For legal
          compliance, FundiLink retains minimal records proving lawful processing and consent.
        </p>

        <section className="bg-white rounded-xl p-5 mb-4 shadow-sm">
          <h2 className="font-semibold text-gray-800 mb-2">Download my data</h2>
          <p className="text-sm text-gray-500 mb-3">Export your profile, applications, documents and consent history.</p>
          <button
            onClick={handleExport}
            disabled={busy}
            className="bg-brand-primary text-white text-sm rounded-lg px-4 py-2 disabled:opacity-50"
          >
            Download export
          </button>
        </section>

        <section className="bg-white rounded-xl p-5 mb-4 shadow-sm">
          <h2 className="font-semibold text-gray-800 mb-2">Request erasure</h2>
          {hasOpenRequest ? (
            <p className="text-sm text-amber-700">You have an erasure request in progress.</p>
          ) : (
            <>
              <p className="text-sm text-gray-500 mb-3">Tell us why (optional), then submit your request.</p>
              <textarea
                aria-label="Erasure reason"
                value={reason}
                onChange={(e) => setReason(e.target.value)}
                className="w-full border rounded-lg p-2 text-sm mb-3"
                rows={3}
              />
              <button
                onClick={handleRequestErasure}
                disabled={busy}
                className="bg-red-600 text-white text-sm rounded-lg px-4 py-2 disabled:opacity-50"
              >
                Request erasure
              </button>
            </>
          )}
        </section>

        <section className="bg-white rounded-xl p-5 shadow-sm">
          <h2 className="font-semibold text-gray-800 mb-3">My requests</h2>
          {requests.length === 0 ? (
            <p className="text-sm text-gray-500">You have no erasure requests.</p>
          ) : (
            <ul className="space-y-2">
              {requests.map((r) => (
                <li key={r.id} className="flex justify-between text-sm border-b pb-2">
                  <span className="text-gray-600">{new Date(r.requestedAt).toLocaleDateString()}</span>
                  <span className="font-medium text-gray-800">{r.status}</span>
                </li>
              ))}
            </ul>
          )}
        </section>
      </div>
    </main>
  )
}

function Centered({ text, className }: { text: string; className: string }) {
  return (
    <main className="min-h-screen bg-brand-light flex items-center justify-center p-4">
      <p className={className}>{text}</p>
    </main>
  )
}
