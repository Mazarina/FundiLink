import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getOperationsDashboard, getPopiaOperationsSummary } from '../features/reporting/reportingApi'
import type { CountByCategory, OperationsDashboard, PopiaOperationsSummary } from '../types'

export default function AdminReportingDashboardPage() {
  const [dashboard, setDashboard] = useState<OperationsDashboard | null>(null)
  const [popia, setPopia] = useState<PopiaOperationsSummary | null>(null)
  const [error, setError] = useState('')

  useEffect(() => {
    Promise.all([getOperationsDashboard(), getPopiaOperationsSummary()])
      .then(([d, p]) => {
        setDashboard(d)
        setPopia(p)
      })
      .catch(() => setError('Could not load the reporting dashboard. Please try again.'))
  }, [])

  if (error) return <Centered text={error} className="text-red-600" />
  if (!dashboard || !popia) return <Centered text="Loading dashboard..." className="text-gray-500" />

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-5xl mx-auto">
        <header className="mb-6">
          <Link to="/profile" className="text-sm text-brand-primary hover:underline">&larr; Back to profile</Link>
          <h1 className="text-2xl font-bold text-brand-primary mt-2">Operations dashboard</h1>
          <p className="text-sm text-gray-500 mt-1">
            Aggregate, read-only figures for staff. No individual learner information is shown here.
          </p>
        </header>

        <section className="grid grid-cols-2 md:grid-cols-4 gap-3 mb-6">
          <StatCard label="Total learners" value={dashboard.totalLearners} />
          <StatCard label="Pending document checks" value={dashboard.pendingDocumentVerifications} />
          <StatCard label="Pending erasures" value={dashboard.pendingErasureRequests} />
          <StatCard label="Consent grants" value={dashboard.consentGrants} />
        </section>

        <section className="bg-white rounded-xl shadow-sm p-5 mb-6">
          <h2 className="text-lg font-semibold text-brand-primary mb-3">POPIA operations</h2>
          <p className="text-sm text-gray-500 mb-4">Open privacy work queues. Action items in their dedicated queues.</p>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <QueueLink
              to="/admin/learners"
              label="Pending document verifications"
              count={popia.pendingDocumentVerifications}
            />
            <QueueLink
              to="/admin/erasure-requests"
              label="Pending erasure requests"
              count={popia.pendingErasureRequests}
            />
          </div>
        </section>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <BreakdownCard title="Learners by province" items={dashboard.learnersByProvince} />
          <BreakdownCard title="Applications by status" items={dashboard.applicationsByStatus} />
          <BreakdownCard title="Bursary applications by status" items={dashboard.bursaryApplicationsByStatus} />
          <BreakdownCard title="Documents by verification status" items={dashboard.documentsByStatus} />
        </div>

        <div className="mt-6">
          <Link to="/admin/audit-activity" className="text-sm text-brand-primary hover:underline">
            View audit activity report &rarr;
          </Link>
        </div>
      </div>
    </main>
  )
}

function StatCard({ label, value }: { label: string; value: number }) {
  return (
    <div className="bg-white rounded-xl shadow-sm p-4">
      <p className="text-2xl font-bold text-brand-primary">{value}</p>
      <p className="text-xs text-gray-500 mt-1">{label}</p>
    </div>
  )
}

function QueueLink({ to, label, count }: { to: string; label: string; count: number }) {
  return (
    <Link to={to} className="flex justify-between items-center border border-gray-100 rounded-lg px-4 py-3 hover:bg-gray-50">
      <span className="text-sm text-gray-700">{label}</span>
      <span className="text-sm font-semibold text-brand-primary">{count}</span>
    </Link>
  )
}

function BreakdownCard({ title, items }: { title: string; items: CountByCategory[] }) {
  return (
    <div className="bg-white rounded-xl shadow-sm p-5">
      <h3 className="text-sm font-semibold text-gray-700 mb-3">{title}</h3>
      {items.length === 0 ? (
        <p className="text-xs text-gray-400">No data.</p>
      ) : (
        <ul className="space-y-1.5">
          {items.map((i) => (
            <li key={i.category} className="flex justify-between text-sm">
              <span className="text-gray-600">{i.category}</span>
              <span className="font-medium text-gray-800">{i.count}</span>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

function Centered({ text, className }: { text: string; className: string }) {
  return (
    <main className="min-h-screen bg-brand-light flex items-center justify-center p-4">
      <p className={className}>{text}</p>
    </main>
  )
}
