import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { searchProgrammes } from '../features/programmes/programmesApi'
import { ProgrammeCard } from '../features/programmes/ProgrammeCard'
import { DisclaimerBanner } from '../features/programmes/DisclaimerBanner'
import type { Programme } from '../types'

const PROVINCES = [
  'Eastern Cape', 'Free State', 'Gauteng', 'KwaZulu-Natal', 'Limpopo',
  'Mpumalanga', 'Northern Cape', 'North West', 'Western Cape',
]

export default function ProgrammesPage() {
  const [keyword, setKeyword] = useState('')
  const [type, setType] = useState('')
  const [province, setProvince] = useState('')
  const [page, setPage] = useState(1)
  const [items, setItems] = useState<Programme[]>([])
  const [total, setTotal] = useState(0)
  const [pageSize, setPageSize] = useState(20)
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    setLoading(true)
    searchProgrammes({
      keyword: keyword || undefined,
      type: type || undefined,
      province: province || undefined,
      page,
    })
      .then((res) => {
        setItems(res.items)
        setTotal(res.total)
        setPageSize(res.pageSize)
      })
      .catch(() => setItems([]))
      .finally(() => setLoading(false))
  }, [keyword, type, province, page])

  const totalPages = Math.max(1, Math.ceil(total / pageSize))

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-3xl mx-auto space-y-4">
        <header className="flex justify-between items-center">
          <h1 className="text-2xl font-bold text-brand-primary">Browse Programmes</h1>
          <Link to="/profile" className="text-sm text-gray-500 hover:underline">Back to profile</Link>
        </header>

        <DisclaimerBanner />

        <div className="bg-white rounded-xl p-4 shadow-sm grid gap-3 sm:grid-cols-3">
          <input
            type="text"
            placeholder="Search programmes or institutions"
            value={keyword}
            onChange={(e) => { setPage(1); setKeyword(e.target.value) }}
            className="border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary sm:col-span-3"
          />
          <select value={type} onChange={(e) => { setPage(1); setType(e.target.value) }}
            className="border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary">
            <option value="">All institution types</option>
            <option value="University">University</option>
            <option value="TVET">TVET</option>
            <option value="SkillsCentre">Skills Centre</option>
          </select>
          <select value={province} onChange={(e) => { setPage(1); setProvince(e.target.value) }}
            className="border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary sm:col-span-2">
            <option value="">All provinces</option>
            {PROVINCES.map((p) => <option key={p} value={p}>{p}</option>)}
          </select>
        </div>

        {loading ? (
          <p className="text-gray-500 text-center py-8">Loading programmes...</p>
        ) : items.length === 0 ? (
          <p className="text-gray-500 text-center py-8">No programmes found. Try adjusting your filters.</p>
        ) : (
          <div className="space-y-3">
            {items.map((p) => (
              <ProgrammeCard key={p.id} id={p.id} name={p.name} institutionName={p.institutionName}
                minimumAps={p.minimumAps} province={p.province} />
            ))}
          </div>
        )}

        {totalPages > 1 && (
          <div className="flex justify-between items-center pt-2">
            <button disabled={page <= 1} onClick={() => setPage((p) => p - 1)}
              className="text-sm text-brand-primary disabled:text-gray-300">← Previous</button>
            <span className="text-sm text-gray-500">Page {page} of {totalPages}</span>
            <button disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)}
              className="text-sm text-brand-primary disabled:text-gray-300">Next →</button>
          </div>
        )}
      </div>
    </main>
  )
}
