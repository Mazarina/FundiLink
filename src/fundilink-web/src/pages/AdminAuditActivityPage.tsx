import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getAuditActivity } from '../features/reporting/reportingApi'
import type { AuditLogEntry } from '../types'

export default function AdminAuditActivityPage() {
  const [entries, setEntries] = useState<AuditLogEntry[]>([])
  const [total, setTotal] = useState(0)
  const [action, setAction] = useState('')
  const [actorRole, setActorRole] = useState('')
  const [error, setError] = useState('')

  function load() {
    getAuditActivity({
      action: action || undefined,
      actorRole: actorRole || undefined,
      page: 1,
      pageSize: 50,
    })
      .then((r) => {
        setEntries(r.items)
        setTotal(r.total)
      })
      .catch(() => setError('Could not load the audit activity report. Please try again.'))
  }

  useEffect(load, [])

  function applyFilters(e: React.FormEvent) {
    e.preventDefault()
    setError('')
    load()
  }

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-5xl mx-auto">
        <header className="mb-6">
          <Link to="/admin/reporting" className="text-sm text-brand-primary hover:underline">&larr; Back to dashboard</Link>
          <h1 className="text-2xl font-bold text-brand-primary mt-2">Audit activity report</h1>
          <p className="text-sm text-gray-500 mt-1">
            Read-only view over the append-only audit log. Filter by action and actor role.
          </p>
        </header>

        <form onSubmit={applyFilters} className="bg-white rounded-xl shadow-sm p-4 mb-4 flex flex-wrap gap-3 items-end">
          <label className="text-sm">
            <span className="block text-gray-600 mb-1">Action</span>
            <input
              aria-label="Filter by action"
              value={action}
              onChange={(e) => setAction(e.target.value)}
              className="border border-gray-200 rounded-lg px-3 py-1.5 text-sm"
              placeholder="e.g. SearchLearners"
            />
          </label>
          <label className="text-sm">
            <span className="block text-gray-600 mb-1">Actor role</span>
            <input
              aria-label="Filter by actor role"
              value={actorRole}
              onChange={(e) => setActorRole(e.target.value)}
              className="border border-gray-200 rounded-lg px-3 py-1.5 text-sm"
              placeholder="e.g. Admin"
            />
          </label>
          <button type="submit" className="bg-brand-primary text-white text-sm rounded-lg px-4 py-1.5">
            Apply filters
          </button>
        </form>

        {error && <p className="text-red-600 text-sm mb-3">{error}</p>}

        <div className="bg-white rounded-xl shadow-sm overflow-x-auto">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 text-gray-600">
              <tr>
                <th className="text-left px-4 py-3">Time</th>
                <th className="text-left px-4 py-3">Actor</th>
                <th className="text-left px-4 py-3">Role</th>
                <th className="text-left px-4 py-3">Action</th>
                <th className="text-left px-4 py-3">Target</th>
              </tr>
            </thead>
            <tbody>
              {entries.map((e) => (
                <tr key={e.id} className="border-t border-gray-100">
                  <td className="px-4 py-2 text-gray-500">{new Date(e.occurredAt).toLocaleString()}</td>
                  <td className="px-4 py-2">{e.actorUserId.slice(0, 8)}…</td>
                  <td className="px-4 py-2">{e.actorRole}</td>
                  <td className="px-4 py-2 font-medium">{e.action}</td>
                  <td className="px-4 py-2 text-gray-500">{e.targetType}/{e.targetId.slice(0, 8)}</td>
                </tr>
              ))}
            </tbody>
          </table>
          {entries.length === 0 && <p className="text-gray-400 text-sm p-4">No audit entries match the filters.</p>}
        </div>
        <p className="text-xs text-gray-400 mt-2">{total} total matching entries.</p>
      </div>
    </main>
  )
}
