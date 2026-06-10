import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import {
  getAccommodationListings,
  trackAccommodationInterest,
} from '../features/accommodation/accommodationApi'
import { AccommodationDisclaimerBanner } from '../features/accommodation/AccommodationDisclaimerBanner'
import type { AccommodationListing, AccommodationType } from '../types'

const ACCOMMODATION_TYPES: AccommodationType[] = [
  'ResidenceOnCampus',
  'PrivateStudentResidence',
  'SharedHouse',
  'Room',
  'Other',
]

export default function AccommodationPage() {
  const [listings, setListings] = useState<AccommodationListing[] | null>(null)
  const [province, setProvince] = useState('')
  const [nearInstitution, setNearInstitution] = useState('')
  const [accommodationType, setAccommodationType] = useState<'' | AccommodationType>('')
  const [error, setError] = useState('')
  const [savedId, setSavedId] = useState('')

  function load() {
    setError('')
    getAccommodationListings({
      province: province || undefined,
      nearInstitution: nearInstitution || undefined,
      accommodationType: accommodationType || undefined,
    })
      .then(setListings)
      .catch(() => setError('Could not load accommodation listings.'))
  }

  useEffect(() => {
    load()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [accommodationType])

  function saveInterest(id: string) {
    trackAccommodationInterest({ accommodationListingId: id, status: 'Saved' })
      .then(() => setSavedId(id))
      .catch(() => setError('Could not save interest.'))
  }

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-3xl mx-auto space-y-4">
        <header className="flex justify-between items-center">
          <h1 className="text-2xl font-bold text-brand-primary">Accommodation</h1>
          <Link to="/accommodation/interests" className="text-sm text-brand-primary hover:underline">My saved</Link>
        </header>

        <AccommodationDisclaimerBanner />

        <div className="flex flex-wrap gap-2">
          <input
            value={province}
            onChange={(e) => setProvince(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && load()}
            placeholder="Province"
            className="border rounded-lg px-3 py-2 text-sm bg-white flex-1 min-w-[120px] focus:outline-none focus:ring-2 focus:ring-brand-primary"
          />
          <input
            value={nearInstitution}
            onChange={(e) => setNearInstitution(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && load()}
            placeholder="Near institution"
            className="border rounded-lg px-3 py-2 text-sm bg-white flex-1 min-w-[120px] focus:outline-none focus:ring-2 focus:ring-brand-primary"
          />
          <select
            value={accommodationType}
            onChange={(e) => setAccommodationType(e.target.value as '' | AccommodationType)}
            className="border rounded-lg px-3 py-2 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-brand-primary"
          >
            <option value="">All types</option>
            {ACCOMMODATION_TYPES.map((t) => (
              <option key={t} value={t}>{t}</option>
            ))}
          </select>
          <button onClick={load} className="bg-brand-primary text-white px-4 py-2 rounded-lg text-sm font-semibold hover:opacity-90">
            Filter
          </button>
        </div>

        {error && <p className="text-red-600 text-sm">{error}</p>}

        {listings === null ? (
          <p className="text-gray-500 text-center py-8">Loading...</p>
        ) : listings.length === 0 ? (
          <div className="bg-white rounded-xl p-6 shadow-sm text-center text-gray-600">
            No accommodation listings found for these filters.
          </div>
        ) : (
          <div className="space-y-3">
            {listings.map((l) => (
              <div key={l.id} className="bg-white rounded-xl p-5 shadow-sm">
                <p className="text-xs text-gray-500">{l.providerName}</p>
                <h3 className="font-semibold text-gray-800">{l.name}</h3>
                <p className="text-sm text-gray-600 mt-1">
                  {l.accommodationType} · {l.city}, {l.province}
                  {l.nearInstitution ? ` · near ${l.nearInstitution}` : ''}
                </p>
                <button
                  onClick={() => saveInterest(l.id)}
                  aria-label={`Save interest in ${l.name}`}
                  disabled={savedId === l.id}
                  className="mt-3 text-sm text-brand-primary border border-brand-primary rounded-lg px-3 py-1 hover:bg-brand-primary hover:text-white disabled:opacity-50"
                >
                  {savedId === l.id ? 'Saved' : 'Save interest'}
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </main>
  )
}
