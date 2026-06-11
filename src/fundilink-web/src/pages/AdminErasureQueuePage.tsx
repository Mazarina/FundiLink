import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import {
  getPendingErasureRequests,
  approveErasureRequest,
  rejectErasureRequest,
  fulfilErasureRequest,
} from '../features/data-rights/dataRightsApi'
import type { ErasureRequest } from '../types'

export default function AdminErasureQueuePage() {
  const [requests, setRequests] = useState<ErasureRequest[] | null>(null)
  const [error, setError] = useState('')
  const [busyId, setBusyId] = useState<string | null>(null)

  function load() {
    getPendingErasureRequests()
      .then(setRequests)
      .catch(() => setError('Could not load the erasure queue. Please try again.'))
  }

  useEffect(load, [])

  async function act(id: string, fn: (id: string, note: string | null) => Promise<void>) {
    setBusyId(id)
    try {
      await fn(id, null)
      load()
    } finally {
      setBusyId(null)
    }
  }

  if (error) return <Centered text={error} className="text-red-600" />
  if (!requests) return <Centered text="Loading erasure queue..." className="text-gray-500" />

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-3xl mx-auto">
        <header className="mb-6">
          <Link to="/profile" className="text-sm text-brand-primary hover:underline">&larr; Back to profile</Link>
          <h1 className="text-2xl font-bold text-brand-primary mt-2">Erasure request queue</h1>
        </header>

        {requests.length === 0 ? (
          <p className="text-sm text-gray-500 bg-white rounded-xl p-5 shadow-sm">No pending erasure requests.</p>
        ) : (
          <ul className="space-y-3">
            {requests.map((r) => (
              <li key={r.id} className="bg-white rounded-xl p-4 shadow-sm">
                <div className="flex justify-between text-sm mb-2">
                  <span className="text-gray-600">Requested {new Date(r.requestedAt).toLocaleDateString()}</span>
                  <span className="font-medium text-gray-800">{r.status}</span>
                </div>
                {r.reason && <p className="text-sm text-gray-500 mb-3">Reason: {r.reason}</p>}
                <div className="flex gap-2">
                  <button
                    aria-label={`Approve ${r.id}`}
                    onClick={() => act(r.id, approveErasureRequest)}
                    disabled={busyId === r.id}
                    className="bg-brand-primary text-white text-xs rounded-lg px-3 py-1.5 disabled:opacity-50"
                  >
                    Approve
                  </button>
                  <button
                    aria-label={`Reject ${r.id}`}
                    onClick={() => act(r.id, rejectErasureRequest)}
                    disabled={busyId === r.id}
                    className="bg-gray-200 text-gray-800 text-xs rounded-lg px-3 py-1.5 disabled:opacity-50"
                  >
                    Reject
                  </button>
                  <button
                    aria-label={`Fulfil ${r.id}`}
                    onClick={() => act(r.id, fulfilErasureRequest)}
                    disabled={busyId === r.id}
                    className="bg-red-600 text-white text-xs rounded-lg px-3 py-1.5 disabled:opacity-50"
                  >
                    Fulfil (anonymise)
                  </button>
                </div>
              </li>
            ))}
          </ul>
        )}
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
