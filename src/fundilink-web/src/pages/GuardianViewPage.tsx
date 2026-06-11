import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getLinkedLearners, getGuardianView } from '../features/consent/consentApi'
import { ConsentDisclaimerBanner, ConsentBadge } from '../features/consent/ConsentDisclaimerBanner'
import type { GuardianView, LinkedLearner } from '../types'

export default function GuardianViewPage() {
  const [learners, setLearners] = useState<LinkedLearner[]>([])
  const [view, setView] = useState<GuardianView | null>(null)
  const [error, setError] = useState('')

  useEffect(() => {
    getLinkedLearners()
      .then(setLearners)
      .catch(() => setError('Could not load linked learners.'))
  }, [])

  async function openLearner(learnerId: string) {
    setError('')
    setView(null)
    try {
      setView(await getGuardianView(learnerId))
    } catch {
      setError('Co-access is not available. The learner must grant guardian co-access consent first.')
    }
  }

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto">
        <header className="mb-6">
          <Link to="/profile" className="text-sm text-brand-primary hover:underline">&larr; Back to profile</Link>
          <h1 className="text-2xl font-bold text-brand-primary mt-2">Guardian Co-Access</h1>
        </header>

        <div className="mb-4">
          <ConsentDisclaimerBanner />
        </div>

        <div className="bg-white rounded-xl p-5 mb-4 shadow-sm">
          <h2 className="font-semibold text-gray-800 mb-3">Learners linked to you</h2>
          {learners.length === 0 ? (
            <p className="text-sm text-gray-500">You are not linked to any learners yet.</p>
          ) : (
            <ul className="space-y-2">
              {learners.map((l) => (
                <li key={l.learnerId} className="flex items-center justify-between gap-3">
                  <div>
                    <p className="text-sm text-gray-800">{l.firstName} {l.surname}</p>
                    <ConsentBadge granted={l.hasCurrentConsent} />
                  </div>
                  <button
                    aria-label={`View ${l.firstName} ${l.surname}`}
                    onClick={() => openLearner(l.learnerId)}
                    className="text-sm text-brand-primary hover:underline"
                  >
                    View
                  </button>
                </li>
              ))}
            </ul>
          )}
        </div>

        {error && <p className="text-red-600 text-sm mb-4">{error}</p>}

        {view && (
          <div className="bg-white rounded-xl p-5 shadow-sm">
            <h2 className="font-semibold text-gray-800 mb-3">{view.firstName} {view.surname}</h2>
            <dl className="grid grid-cols-2 gap-x-4 gap-y-2 text-sm mb-3">
              <dt className="text-gray-500">Grade</dt><dd className="text-gray-800">{view.gradeLevel}</dd>
              <dt className="text-gray-500">School</dt><dd className="text-gray-800">{view.schoolName}</dd>
              <dt className="text-gray-500">Province</dt><dd className="text-gray-800">{view.province}</dd>
              <dt className="text-gray-500">Profile completeness</dt><dd className="text-gray-800">{view.profileCompleteness}%</dd>
            </dl>
            {view.scope === 'ProfileAndApplications' && (
              <div>
                <h3 className="font-medium text-gray-700 text-sm mb-2">Applications</h3>
                {view.applications.length === 0 ? (
                  <p className="text-sm text-gray-500">No applications tracked.</p>
                ) : (
                  <ul className="space-y-1 text-sm">
                    {view.applications.map((a, i) => (
                      <li key={i} className="flex justify-between">
                        <span className="text-gray-800">{a.programmeOrBursaryName} ({a.kind})</span>
                        <span className="text-gray-500">{a.status}</span>
                      </li>
                    ))}
                  </ul>
                )}
              </div>
            )}
          </div>
        )}
      </div>
    </main>
  )
}
