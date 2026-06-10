import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import {
  getAccommodationInterests,
  updateAccommodationInterestStatus,
} from '../features/accommodation/accommodationApi'
import { AccommodationDisclaimerBanner } from '../features/accommodation/AccommodationDisclaimerBanner'
import type { AccommodationInterest, OpportunityInterestStatus } from '../types'

const STATUSES: OpportunityInterestStatus[] = ['Saved', 'Contacted', 'Applied', 'NotInterested']

export default function AccommodationInterestsPage() {
  const [interests, setInterests] = useState<AccommodationInterest[] | null>(null)
  const [error, setError] = useState('')

  function load() {
    getAccommodationInterests()
      .then(setInterests)
      .catch(() => setError('Could not load your saved accommodation.'))
  }

  useEffect(() => {
    load()
  }, [])

  async function handleStatusChange(id: string, newStatus: OpportunityInterestStatus) {
    setError('')
    try {
      await updateAccommodationInterestStatus(id, { newStatus })
      load()
    } catch {
      setError('Could not update status. Please try again.')
    }
  }

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto space-y-4">
        <header className="flex justify-between items-center">
          <h1 className="text-2xl font-bold text-brand-primary">My Saved Accommodation</h1>
          <Link to="/accommodation" className="text-sm text-brand-primary hover:underline">Browse accommodation</Link>
        </header>

        <AccommodationDisclaimerBanner />

        {error && <p className="text-red-600 text-sm">{error}</p>}

        {interests === null ? (
          <p className="text-gray-500 text-center py-8">Loading...</p>
        ) : interests.length === 0 ? (
          <div className="bg-white rounded-xl p-6 shadow-sm text-center text-gray-600">
            No accommodation saved yet. Browse accommodation to get started.
          </div>
        ) : (
          <div className="space-y-3">
            {interests.map((i) => (
              <div key={i.id} className="bg-white rounded-xl p-5 shadow-sm space-y-3">
                <div className="min-w-0">
                  <p className="text-xs text-gray-500 truncate">{i.providerName}</p>
                  <h3 className="font-semibold text-gray-800">{i.listingName}</h3>
                </div>
                <div className="flex items-center gap-2">
                  <label htmlFor={`status-${i.id}`} className="text-xs text-gray-500">Status</label>
                  <select
                    id={`status-${i.id}`}
                    aria-label={`Status for ${i.listingName}`}
                    value={i.status}
                    onChange={(e) => handleStatusChange(i.id, e.target.value as OpportunityInterestStatus)}
                    className="border rounded-lg px-2 py-1 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-brand-primary"
                  >
                    {STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
                  </select>
                </div>
                {i.contactUrl && (
                  <a href={i.contactUrl} target="_blank" rel="noopener noreferrer"
                    className="text-xs text-brand-primary hover:underline">
                    Provider's official channel →
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
