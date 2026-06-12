import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { searchLearners } from '../features/admin/adminApi'
import type { LearnerSummary } from '../types'
import { gradeLevelLabel } from '../utils/format'

export default function AdminLearnersPage() {
  const navigate = useNavigate()
  const [keyword, setKeyword] = useState('')
  const [results, setResults] = useState<LearnerSummary[]>([])
  const [searched, setSearched] = useState(false)

  async function handleSearch(e: React.FormEvent) {
    e.preventDefault()
    const res = await searchLearners({ keyword })
    setResults(res.items)
    setSearched(true)
  }

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-4xl mx-auto space-y-4">
        <h1 className="text-2xl font-bold text-brand-primary">Learner Search</h1>
        <form onSubmit={handleSearch} className="flex gap-2">
          <input value={keyword} onChange={e => setKeyword(e.target.value)} placeholder="Search by name..."
            className="flex-1 border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary" />
          <button type="submit" className="bg-brand-primary text-white px-4 py-2 rounded-lg text-sm font-semibold">Search</button>
        </form>
        {searched && results.length === 0 && <p className="text-gray-500 text-sm">No learners found.</p>}
        <div className="space-y-2">
          {results.map(l => (
            <div key={l.id} className="bg-white rounded-xl p-4 shadow-sm flex justify-between items-center cursor-pointer hover:bg-gray-50"
              onClick={() => navigate(`/admin/learners/${l.id}`)}>
              <div>
                <div className="font-semibold text-gray-800">{l.fullName}</div>
                <div className="text-xs text-gray-500">{l.province} · {gradeLevelLabel(l.gradeLevel)}</div>
              </div>
              <div className="text-sm text-gray-600">Profile: {l.profileCompleteness}%</div>
            </div>
          ))}
        </div>
      </div>
    </main>
  )
}
