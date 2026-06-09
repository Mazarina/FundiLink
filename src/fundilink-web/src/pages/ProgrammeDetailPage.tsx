import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { getProgramme } from '../features/programmes/programmesApi'
import { createApplication } from '../features/applications/applicationsApi'
import { DisclaimerBanner } from '../features/programmes/DisclaimerBanner'
import type { Programme } from '../types'

export default function ProgrammeDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [programme, setProgramme] = useState<Programme | null>(null)
  const [error, setError] = useState('')
  const [tracking, setTracking] = useState(false)

  useEffect(() => {
    if (!id) return
    getProgramme(id)
      .then(setProgramme)
      .catch(() => setError('Could not load this programme.'))
  }, [id])

  async function handleTrack() {
    if (!id) return
    setTracking(true)
    setError('')
    try {
      await createApplication({ programmeId: id, status: 'Interested' })
      navigate('/applications')
    } catch {
      setError('Could not start tracking this application. Please try again.')
      setTracking(false)
    }
  }

  if (error && !programme) return (
    <main className="min-h-screen bg-brand-light flex items-center justify-center p-4">
      <p className="text-red-600">{error}</p>
    </main>
  )

  if (!programme) return (
    <main className="min-h-screen bg-brand-light flex items-center justify-center p-4">
      <p className="text-gray-500">Loading programme...</p>
    </main>
  )

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto space-y-4">
        <Link to="/programmes" className="text-sm text-gray-500 hover:underline">← Back to programmes</Link>

        <DisclaimerBanner />

        <div className="bg-white rounded-xl p-6 shadow-sm space-y-3">
          <p className="text-sm text-gray-500">{programme.institutionName}</p>
          <h1 className="text-2xl font-bold text-brand-primary">{programme.name}</h1>
          <dl className="grid grid-cols-2 gap-x-4 gap-y-2 text-sm">
            <dt className="text-gray-500">Province</dt>
            <dd className="font-medium text-gray-800">{programme.province}</dd>
            <dt className="text-gray-500">Institution type</dt>
            <dd className="font-medium text-gray-800">{programme.institutionType}</dd>
            <dt className="text-gray-500">Minimum APS</dt>
            <dd className="font-medium text-gray-800">{programme.minimumAps}</dd>
            {programme.nfqLevel != null && (
              <>
                <dt className="text-gray-500">NQF level</dt>
                <dd className="font-medium text-gray-800">{programme.nfqLevel}</dd>
              </>
            )}
          </dl>
        </div>

        <div className="bg-white rounded-xl p-6 shadow-sm">
          <h2 className="font-semibold text-gray-800 mb-3">Required subjects</h2>
          {programme.requiredSubjects && programme.requiredSubjects.length > 0 ? (
            <ul className="space-y-2 text-sm">
              {programme.requiredSubjects.map((s) => (
                <li key={s.subjectName} className="flex justify-between border-b border-gray-100 pb-1">
                  <span className="text-gray-700">{s.subjectName}</span>
                  <span className="font-medium text-gray-800">{s.minimumPercentage}%</span>
                </li>
              ))}
            </ul>
          ) : (
            <p className="text-sm text-gray-500">No specific subject requirements listed.</p>
          )}
        </div>

        {error && <p className="text-sm text-red-600">{error}</p>}

        <button onClick={handleTrack} disabled={tracking}
          className="w-full bg-brand-primary text-white py-3 rounded-lg font-semibold hover:opacity-90 disabled:opacity-50">
          {tracking ? 'Adding...' : 'Track Application'}
        </button>
      </div>
    </main>
  )
}
