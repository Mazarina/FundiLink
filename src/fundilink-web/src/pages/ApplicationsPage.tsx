import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getApplications } from '../features/applications/applicationsApi'
import { ApplicationStatusBadge } from '../features/applications/ApplicationStatusBadge'
import type { LearnerApplication } from '../types'

export default function ApplicationsPage() {
  const [applications, setApplications] = useState<LearnerApplication[] | null>(null)
  const [error, setError] = useState('')

  useEffect(() => {
    getApplications()
      .then(setApplications)
      .catch(() => setError('Could not load your applications.'))
  }, [])

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto space-y-4">
        <header className="flex justify-between items-center">
          <h1 className="text-2xl font-bold text-brand-primary">My Applications</h1>
          <Link to="/programmes" className="text-sm text-brand-primary hover:underline">Browse programmes</Link>
        </header>

        {error && <p className="text-red-600 text-sm">{error}</p>}

        {applications === null ? (
          <p className="text-gray-500 text-center py-8">Loading...</p>
        ) : applications.length === 0 ? (
          <div className="bg-white rounded-xl p-6 shadow-sm text-center text-gray-600">
            No applications tracked yet. Browse programmes to get started.
          </div>
        ) : (
          <div className="space-y-3">
            {applications.map((a) => (
              <Link key={a.id} to={`/applications/${a.id}`}
                className="block bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition">
                <div className="flex justify-between items-start gap-3">
                  <div className="min-w-0">
                    <p className="text-xs text-gray-500 truncate">{a.institutionName}</p>
                    <h3 className="font-semibold text-gray-800">{a.programmeName}</h3>
                    {a.deadlineDate && (
                      <p className="text-xs text-gray-400 mt-1">
                        Deadline: {new Date(a.deadlineDate).toLocaleDateString()}
                      </p>
                    )}
                  </div>
                  <ApplicationStatusBadge status={a.status} />
                </div>
              </Link>
            ))}
          </div>
        )}
      </div>
    </main>
  )
}
