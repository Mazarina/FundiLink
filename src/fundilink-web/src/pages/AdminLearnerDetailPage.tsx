import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { getLearnerOverview } from '../features/admin/adminApi'
import type { LearnerOverview } from '../types'

export default function AdminLearnerDetailPage() {
  const { id } = useParams<{ id: string }>()
  const [overview, setOverview] = useState<LearnerOverview | null>(null)

  useEffect(() => {
    if (id) getLearnerOverview(id).then(setOverview).catch(() => {})
  }, [id])

  if (!overview) return <main className="min-h-screen bg-brand-light flex items-center justify-center"><p className="text-gray-500">Loading...</p></main>

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto space-y-4">
        <h1 className="text-2xl font-bold text-brand-primary">{overview.summary.fullName}</h1>
        <div className="bg-white rounded-xl p-4 shadow-sm grid grid-cols-3 gap-4 text-center">
          <div><div className="text-2xl font-bold text-brand-primary">{overview.apsScore}</div><div className="text-xs text-gray-500">APS Score</div></div>
          <div><div className="text-2xl font-bold text-brand-primary">{overview.applicationCount}</div><div className="text-xs text-gray-500">Applications</div></div>
          <div><div className="text-2xl font-bold text-brand-primary">{overview.documentCount}</div><div className="text-xs text-gray-500">Documents</div></div>
        </div>
      </div>
    </main>
  )
}
