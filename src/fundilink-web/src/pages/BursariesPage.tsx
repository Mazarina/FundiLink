import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getBursaries } from '../features/bursaries/bursariesApi'
import { BursaryDisclaimerBanner } from '../features/bursaries/BursaryDisclaimerBanner'
import type { Bursary, BursaryFundingType } from '../types'

const FUNDING_TYPES: BursaryFundingType[] = ['FullCost', 'TuitionOnly', 'PartialTuition', 'Stipend', 'Accommodation']

export default function BursariesPage() {
  const [bursaries, setBursaries] = useState<Bursary[] | null>(null)
  const [fieldOfStudy, setFieldOfStudy] = useState('')
  const [province, setProvince] = useState('')
  const [fundingType, setFundingType] = useState<'' | BursaryFundingType>('')
  const [error, setError] = useState('')

  function load() {
    setError('')
    getBursaries({
      fieldOfStudy: fieldOfStudy || undefined,
      province: province || undefined,
      fundingType: fundingType || undefined,
    })
      .then(setBursaries)
      .catch(() => setError('Could not load bursaries.'))
  }

  useEffect(() => {
    load()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [fundingType])

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-3xl mx-auto space-y-4">
        <header className="flex justify-between items-center">
          <h1 className="text-2xl font-bold text-brand-primary">Bursary Hub</h1>
          <Link to="/bursaries/matches" className="text-sm text-brand-primary hover:underline">My matches</Link>
        </header>

        <BursaryDisclaimerBanner />

        <div className="flex flex-wrap gap-2">
          <input
            value={fieldOfStudy}
            onChange={(e) => setFieldOfStudy(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && load()}
            placeholder="Field of study"
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
            value={fundingType}
            onChange={(e) => setFundingType(e.target.value as '' | BursaryFundingType)}
            className="border rounded-lg px-3 py-2 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-brand-primary"
          >
            <option value="">All funding types</option>
            {FUNDING_TYPES.map((t) => (
              <option key={t} value={t}>{t}</option>
            ))}
          </select>
          <button onClick={load} className="bg-brand-primary text-white px-4 py-2 rounded-lg text-sm font-semibold hover:opacity-90">
            Filter
          </button>
        </div>

        {error && <p className="text-red-600 text-sm">{error}</p>}

        {bursaries === null ? (
          <p className="text-gray-500 text-center py-8">Loading...</p>
        ) : bursaries.length === 0 ? (
          <div className="bg-white rounded-xl p-6 shadow-sm text-center text-gray-600">
            No bursaries found for these filters.
          </div>
        ) : (
          <div className="space-y-3">
            {bursaries.map((b) => (
              <Link key={b.id} to={`/bursaries/${b.id}`}
                className="block bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition">
                <p className="text-xs text-gray-500">{b.providerName}</p>
                <h3 className="font-semibold text-gray-800">{b.name}</h3>
                <p className="text-sm text-gray-600 mt-1">
                  {b.fundingType}{b.minimumAps != null ? ` · Min APS ${b.minimumAps}` : ''}
                </p>
              </Link>
            ))}
          </div>
        )}
      </div>
    </main>
  )
}
