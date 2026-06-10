import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getBursaryMatches } from '../features/bursaries/bursariesApi'
import { BursaryDisclaimerBanner } from '../features/bursaries/BursaryDisclaimerBanner'
import type { BursaryMatch } from '../types'

export default function BursaryMatchesPage() {
  const [matches, setMatches] = useState<BursaryMatch[] | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    getBursaryMatches()
      .then(setMatches)
      .catch(() => setError('Could not load bursary matches.'))
      .finally(() => setLoading(false))
  }, [])

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-3xl mx-auto space-y-4">
        <header className="flex justify-between items-center">
          <h1 className="text-2xl font-bold text-brand-primary">Bursary Matches</h1>
          <Link to="/bursaries" className="text-sm text-gray-500 hover:underline">Browse all bursaries</Link>
        </header>

        <BursaryDisclaimerBanner />

        {error && <p className="text-red-600 text-sm">{error}</p>}

        {loading ? (
          <p className="text-gray-500 text-center py-8">Finding bursaries you may qualify for...</p>
        ) : matches !== null && matches.length === 0 ? (
          <div className="bg-white rounded-xl p-6 shadow-sm text-center">
            <p className="text-gray-700 mb-3">
              We could not find bursary matches yet. Complete your academic profile so we can use your APS.
            </p>
            <Link to="/academic" className="text-brand-primary font-semibold hover:underline">
              Complete academic profile →
            </Link>
          </div>
        ) : (
          <div className="space-y-3">
            {(matches ?? []).map((m) => (
              <Link key={m.bursaryId} to={`/bursaries/${m.bursaryId}`}
                className="block bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition">
                <div className="flex justify-between items-start gap-3">
                  <div className="min-w-0">
                    <p className="text-xs text-gray-500 truncate">{m.providerName}</p>
                    <h3 className="font-semibold text-gray-800">{m.name}</h3>
                  </div>
                  <span className="shrink-0 bg-green-100 text-green-700 text-xs font-semibold px-2 py-1 rounded-full">
                    You may qualify
                  </span>
                </div>
                {m.reasons.length > 0 && (
                  <ul className="text-xs text-gray-600 mt-2 list-disc list-inside">
                    {m.reasons.map((r) => <li key={r}>{r}</li>)}
                  </ul>
                )}
              </Link>
            ))}
          </div>
        )}
      </div>
    </main>
  )
}
