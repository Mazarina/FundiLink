import { useEffect, useState } from 'react'
import { getAuditLog } from '../features/admin/adminApi'
import type { AuditLogEntry } from '../types'

export default function AuditLogPage() {
  const [entries, setEntries] = useState<AuditLogEntry[]>([])

  useEffect(() => { getAuditLog({}).then(r => setEntries(r.items)).catch(() => {}) }, [])

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-5xl mx-auto">
        <h1 className="text-2xl font-bold text-brand-primary mb-4">Audit Log</h1>
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
              {entries.map(e => (
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
          {entries.length === 0 && <p className="text-gray-400 text-sm p-4">No audit entries.</p>}
        </div>
      </div>
    </main>
  )
}
