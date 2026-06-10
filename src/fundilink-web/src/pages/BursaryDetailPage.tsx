import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { getBursary, createBursaryApplication } from '../features/bursaries/bursariesApi'
import { BursaryDisclaimerBanner } from '../features/bursaries/BursaryDisclaimerBanner'
import type { Bursary } from '../types'

export default function BursaryDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [bursary, setBursary] = useState<Bursary | null>(null)
  const [error, setError] = useState('')
  const [tracking, setTracking] = useState(false)

  useEffect(() => {
    if (!id) return
    getBursary(id)
      .then(setBursary)
      .catch(() => setError('Could not load this bursary.'))
  }, [id])

  async function handleTrack() {
    if (!id) return
    setTracking(true)
    setError('')
    try {
      await createBursaryApplication({ bursaryId: id, status: 'Researching' })
      navigate('/bursary-applications')
    } catch {
      setError('Could not start tracking this bursary. Please try again.')
      setTracking(false)
    }
  }

  if (error && !bursary) return (
    <main className="min-h-screen bg-brand-light flex items-center justify-center p-4">
      <p className="text-red-600">{error}</p>
    </main>
  )

  if (!bursary) return (
    <main className="min-h-screen bg-brand-light flex items-center justify-center p-4">
      <p className="text-gray-500">Loading bursary...</p>
    </main>
  )

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto space-y-4">
        <Link to="/bursaries" className="text-sm text-gray-500 hover:underline">← Back to bursaries</Link>

        <BursaryDisclaimerBanner />

        <div className="bg-white rounded-xl p-6 shadow-sm space-y-3">
          <p className="text-sm text-gray-500">{bursary.providerName}</p>
          <h1 className="text-2xl font-bold text-brand-primary">{bursary.name}</h1>
          <p className="text-sm text-gray-700">{bursary.description}</p>
          <dl className="grid grid-cols-2 gap-x-4 gap-y-2 text-sm">
            <dt className="text-gray-500">Funding type</dt>
            <dd className="font-medium text-gray-800">{bursary.fundingType}</dd>
            {bursary.minimumAps != null && (
              <>
                <dt className="text-gray-500">Minimum APS</dt>
                <dd className="font-medium text-gray-800">{bursary.minimumAps}</dd>
              </>
            )}
            {bursary.fieldsOfStudy.length > 0 && (
              <>
                <dt className="text-gray-500">Fields of study</dt>
                <dd className="font-medium text-gray-800">{bursary.fieldsOfStudy.join(', ')}</dd>
              </>
            )}
            {bursary.provincesEligible.length > 0 && (
              <>
                <dt className="text-gray-500">Provinces</dt>
                <dd className="font-medium text-gray-800">{bursary.provincesEligible.join(', ')}</dd>
              </>
            )}
          </dl>
        </div>

        <div className="bg-amber-50 border border-amber-200 text-amber-800 text-sm rounded-lg p-4">
          This is guidance only. Apply on the funder's official portal.
          {bursary.externalApplicationUrl && (
            <a href={bursary.externalApplicationUrl} target="_blank" rel="noopener noreferrer"
              className="block mt-2 font-semibold text-brand-primary hover:underline">
              Go to the funder's official portal →
            </a>
          )}
        </div>

        {error && <p className="text-sm text-red-600">{error}</p>}

        <button onClick={handleTrack} disabled={tracking}
          className="w-full bg-brand-primary text-white py-3 rounded-lg font-semibold hover:opacity-90 disabled:opacity-50">
          {tracking ? 'Adding...' : 'Track This Bursary'}
        </button>
      </div>
    </main>
  )
}
