import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getMatches } from '../features/programmes/programmesApi'
import { DisclaimerBanner } from '../features/programmes/DisclaimerBanner'
import type { ProgrammeMatch } from '../types'

export default function MatchesPage() {
  const [matches, setMatches] = useState<ProgrammeMatch[] | null>(null)
  const [type, setType] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    getMatches()
      .then(setMatches)
      .catch(() => setError('Could not load matches.'))
      .finally(() => setLoading(false))
  }, [])

  const filtered = (matches ?? []).filter((m) => !type || m.institutionType === type)

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-3xl mx-auto space-y-4">
        <header className="flex justify-between items-center">
          <h1 className="text-2xl font-bold text-brand-primary">My Matches</h1>
          <Link to="/profile" className="text-sm text-gray-500 hover:underline">Back to profile</Link>
        </header>

        <DisclaimerBanner />

        {error && <p className="text-red-600 text-sm">{error}</p>}

        {loading ? (
          <p className="text-gray-500 text-center py-8">Finding your matches...</p>
        ) : matches !== null && matches.length === 0 ? (
          <div className="bg-white rounded-xl p-6 shadow-sm text-center">
            <p className="text-gray-700 mb-3">
              We could not find matches yet. Complete your academic profile so we can calculate your APS.
            </p>
            <Link to="/academic" className="text-brand-primary font-semibold hover:underline">
              Complete academic profile →
            </Link>
          </div>
        ) : (
          <>
            <select value={type} onChange={(e) => setType(e.target.value)}
              className="border rounded-lg px-3 py-2 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-brand-primary">
              <option value="">All institution types</option>
              <option value="University">University</option>
              <option value="TVET">TVET</option>
              <option value="SkillsCentre">Skills Centre</option>
            </select>

            <div className="space-y-3">
              {filtered.map((m) => (
                <Link key={m.id} to={`/programmes/${m.id}`}
                  className="block bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition">
                  <div className="flex justify-between items-start gap-3">
                    <div className="min-w-0">
                      <p className="text-xs text-gray-500 truncate">{m.institutionName}</p>
                      <h3 className="font-semibold text-gray-800">{m.name}</h3>
                      <p className="text-sm text-gray-600 mt-1">Minimum APS: {m.minimumAps}</p>
                    </div>
                    {m.isEligible ? (
                      <span className="shrink-0 bg-green-100 text-green-700 text-xs font-semibold px-2 py-1 rounded-full">
                        ✓ Eligible
                      </span>
                    ) : m.apsGap <= 5 ? (
                      <span className="shrink-0 bg-amber-100 text-amber-700 text-xs font-semibold px-2 py-1 rounded-full">
                        Almost ({m.apsGap} APS away)
                      </span>
                    ) : (
                      <span className="shrink-0 bg-gray-100 text-gray-600 text-xs font-semibold px-2 py-1 rounded-full">
                        {m.apsGap} APS away
                      </span>
                    )}
                  </div>
                  {m.missingSubjects.length > 0 && (
                    <p className="text-xs text-red-500 mt-2">
                      Missing subject requirements: {m.missingSubjects.join(', ')}
                    </p>
                  )}
                </Link>
              ))}
            </div>
          </>
        )}
      </div>
    </main>
  )
}
