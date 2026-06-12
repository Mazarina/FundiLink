import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getHomeSummary } from '../features/home/homeApi'
import type { LearnerHomeSummary } from '../types'

const KIND_LABELS: Record<string, string> = {
  ProgrammeApplication: 'Programme',
  BursaryApplication: 'Bursary',
}

const NOTIFICATION_TYPE_LABELS: Record<string, string> = {
  DeadlineReminder: 'Deadline reminder',
  ApplicationStatusChange: 'Application status update',
  DocumentVerificationResult: 'Document verification',
  RegistrationWelcome: 'Welcome',
  BursaryStatusChange: 'Bursary status update',
}

function SummaryCard({
  title,
  to,
  linkLabel,
  children,
}: {
  title: string
  to: string
  linkLabel: string
  children: React.ReactNode
}) {
  return (
    <section className="bg-white rounded-xl p-4 shadow-sm flex flex-col">
      <h2 className="text-lg font-semibold text-brand-primary mb-2">{title}</h2>
      <div className="flex-1 text-sm text-gray-700">{children}</div>
      <Link to={to} className="mt-3 text-sm font-medium text-brand-accent hover:underline">
        {linkLabel} →
      </Link>
    </section>
  )
}

export default function HomeDashboardPage() {
  const [summary, setSummary] = useState<LearnerHomeSummary | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    getHomeSummary()
      .then(setSummary)
      .catch(() => setError('Could not load your dashboard.'))
      .finally(() => setLoading(false))
  }, [])

  if (loading)
    return (
      <main className="min-h-screen bg-brand-light flex items-center justify-center p-4">
        <p className="text-gray-500">Loading your dashboard...</p>
      </main>
    )

  if (error)
    return (
      <main className="min-h-screen bg-brand-light p-4">
        <div className="max-w-3xl mx-auto">
          <div className="p-3 bg-red-50 border border-red-200 rounded text-red-700 text-sm">{error}</div>
        </div>
      </main>
    )

  if (!summary) return null

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-3xl mx-auto space-y-4">
        <header>
          <h1 className="text-2xl font-bold text-brand-primary">
            Welcome back, {summary.firstName}
          </h1>
          <p className="text-sm text-gray-500">
            Your progress at a glance. FundiLink helps you prepare, organise and track —
            it is not an official admissions or funding portal.
          </p>
        </header>

        <div className="grid gap-4 sm:grid-cols-2">
          <SummaryCard title="Profile" to="/profile" linkLabel="View profile">
            <p>
              <span className="text-3xl font-bold text-brand-primary">
                {summary.profileCompleteness}%
              </span>{' '}
              complete
            </p>
          </SummaryCard>

          <SummaryCard
            title="Programme applications"
            to="/applications"
            linkLabel="Manage applications"
          >
            <p className="mb-1 font-medium">{summary.programmeApplicationTotal} total</p>
            {summary.programmeApplicationCounts.length === 0 ? (
              <p className="text-gray-500">No programme applications yet.</p>
            ) : (
              <ul className="space-y-0.5">
                {summary.programmeApplicationCounts.map((c) => (
                  <li key={c.status}>
                    {c.status}: {c.count}
                  </li>
                ))}
              </ul>
            )}
          </SummaryCard>

          <SummaryCard title="Bursary applications" to="/bursary-applications" linkLabel="Manage bursaries">
            <p className="mb-1 font-medium">{summary.bursaryApplicationTotal} total</p>
            {summary.bursaryApplicationCounts.length === 0 ? (
              <p className="text-gray-500">No bursary applications yet.</p>
            ) : (
              <ul className="space-y-0.5">
                {summary.bursaryApplicationCounts.map((c) => (
                  <li key={c.status}>
                    {c.status}: {c.count}
                  </li>
                ))}
              </ul>
            )}
          </SummaryCard>

          <SummaryCard title="Documents" to="/documents" linkLabel="Prepare documents">
            <p>
              <span className="text-3xl font-bold text-brand-primary">
                {summary.pendingDocumentCount}
              </span>{' '}
              required document{summary.pendingDocumentCount === 1 ? '' : 's'} still needed
            </p>
          </SummaryCard>
        </div>

        <SummaryCard title="Upcoming deadlines" to="/applications" linkLabel="See all deadlines">
          {summary.upcomingDeadlines.length === 0 ? (
            <p className="text-gray-500">No deadlines in the next 30 days.</p>
          ) : (
            <ul className="space-y-2">
              {summary.upcomingDeadlines.map((d, i) => (
                <li key={`${d.opportunityName}-${i}`} className="flex justify-between">
                  <span>
                    <span className="text-xs text-gray-500">
                      {KIND_LABELS[d.kind] ?? d.kind}
                    </span>{' '}
                    {d.opportunityName}
                  </span>
                  <span className="text-gray-600">
                    {new Date(d.deadlineDate).toLocaleDateString()}
                  </span>
                </li>
              ))}
            </ul>
          )}
        </SummaryCard>

        <SummaryCard
          title="Recent notifications"
          to="/notifications/history"
          linkLabel="View notification history"
        >
          {summary.recentNotifications.length === 0 ? (
            <p className="text-gray-500">No recent notifications.</p>
          ) : (
            <ul className="space-y-1">
              {summary.recentNotifications.map((n) => (
                <li key={n.id} className="flex justify-between">
                  <span>{NOTIFICATION_TYPE_LABELS[n.notificationType] ?? n.notificationType}</span>
                  <span className="text-gray-500">{new Date(n.sentAt).toLocaleDateString()}</span>
                </li>
              ))}
            </ul>
          )}
        </SummaryCard>
      </div>
    </main>
  )
}
