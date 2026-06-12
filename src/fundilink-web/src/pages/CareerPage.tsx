import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import {
  getCareerOpportunities,
  trackCareerInterest,
} from '../features/career/careerApi'
import { CareerDisclaimerBanner } from '../features/career/CareerDisclaimerBanner'
import type { CareerOpportunity, CareerOpportunityType } from '../types'
import { humanizeEnum } from '../utils/format'

const OPPORTUNITY_TYPES: CareerOpportunityType[] = [
  'Learnership',
  'Internship',
  'SkillsProgramme',
  'Apprenticeship',
  'EntryLevelJob',
]

export default function CareerPage() {
  const [opportunities, setOpportunities] = useState<CareerOpportunity[] | null>(null)
  const [fieldOfInterest, setFieldOfInterest] = useState('')
  const [province, setProvince] = useState('')
  const [opportunityType, setOpportunityType] = useState<'' | CareerOpportunityType>('')
  const [error, setError] = useState('')
  const [savedId, setSavedId] = useState('')

  function load() {
    setError('')
    getCareerOpportunities({
      fieldOfInterest: fieldOfInterest || undefined,
      province: province || undefined,
      opportunityType: opportunityType || undefined,
    })
      .then(setOpportunities)
      .catch(() => setError('Could not load career opportunities.'))
  }

  useEffect(() => {
    load()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [opportunityType])

  function saveInterest(id: string) {
    trackCareerInterest({ careerOpportunityId: id, status: 'Saved' })
      .then(() => setSavedId(id))
      .catch(() => setError('Could not track interest.'))
  }

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-3xl mx-auto space-y-4">
        <header className="flex justify-between items-center">
          <h1 className="text-2xl font-bold text-brand-primary">Career Opportunities</h1>
          <Link to="/career/interests" className="text-sm text-brand-primary hover:underline">My tracked</Link>
        </header>

        <CareerDisclaimerBanner />

        <div className="flex flex-wrap gap-2">
          <input
            value={fieldOfInterest}
            onChange={(e) => setFieldOfInterest(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && load()}
            placeholder="Field of interest"
            className="border rounded-lg px-3 py-2 text-sm bg-white flex-1 min-w-[120px] focus:outline-none focus:ring-2 focus:ring-brand-primary"
          />
          <input
            value={province}
            onChange={(e) => setProvince(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && load()}
            placeholder="Province"
            className="border rounded-lg px-3 py-2 text-sm bg-white flex-1 min-w-[120px] focus:outline-none focus:ring-2 focus:ring-brand-primary"
          />
          <select
            value={opportunityType}
            onChange={(e) => setOpportunityType(e.target.value as '' | CareerOpportunityType)}
            className="border rounded-lg px-3 py-2 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-brand-primary"
          >
            <option value="">All types</option>
            {OPPORTUNITY_TYPES.map((t) => (
              <option key={t} value={t}>{humanizeEnum(t)}</option>
            ))}
          </select>
          <button onClick={load} className="bg-brand-primary text-white px-4 py-2 rounded-lg text-sm font-semibold hover:opacity-90">
            Filter
          </button>
        </div>

        {error && <p className="text-red-600 text-sm">{error}</p>}

        {opportunities === null ? (
          <p className="text-gray-500 text-center py-8">Loading...</p>
        ) : opportunities.length === 0 ? (
          <div className="bg-white rounded-xl p-6 shadow-sm text-center text-gray-600">
            No career opportunities found for these filters.
          </div>
        ) : (
          <div className="space-y-3">
            {opportunities.map((o) => (
              <div key={o.id} className="bg-white rounded-xl p-5 shadow-sm">
                <p className="text-xs text-gray-500">{o.providerName}</p>
                <h3 className="font-semibold text-gray-800">{o.title}</h3>
                <p className="text-sm text-gray-600 mt-1">
                  {humanizeEnum(o.opportunityType)}
                  {o.fieldsOfInterest.length > 0 ? ` · ${o.fieldsOfInterest.join(', ')}` : ''}
                </p>
                {o.applicationCloseDate && (
                  <p className="text-xs text-gray-500 mt-1">
                    Closes {new Date(o.applicationCloseDate).toLocaleDateString()}
                  </p>
                )}
                <button
                  onClick={() => saveInterest(o.id)}
                  aria-label={`Track interest in ${o.title}`}
                  disabled={savedId === o.id}
                  className="mt-3 text-sm text-brand-primary border border-brand-primary rounded-lg px-3 py-1 hover:bg-brand-primary hover:text-white disabled:opacity-50"
                >
                  {savedId === o.id ? 'Tracked' : 'Track interest'}
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </main>
  )
}
