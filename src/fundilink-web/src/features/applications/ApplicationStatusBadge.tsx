import type { ApplicationStatus } from '../../types'

const STATUS_STYLES: Record<ApplicationStatus, string> = {
  Interested: 'bg-gray-100 text-gray-700',
  InProgress: 'bg-blue-100 text-blue-700',
  Submitted: 'bg-yellow-100 text-yellow-700',
  Accepted: 'bg-green-100 text-green-700',
  Rejected: 'bg-red-100 text-red-700',
  Waitlisted: 'bg-orange-100 text-orange-700',
}

const STATUS_LABELS: Record<ApplicationStatus, string> = {
  Interested: 'Interested',
  InProgress: 'In Progress',
  Submitted: 'Submitted',
  Accepted: 'Accepted',
  Rejected: 'Rejected',
  Waitlisted: 'Waitlisted',
}

export function ApplicationStatusBadge({ status }: { status: ApplicationStatus }) {
  return (
    <span
      data-testid="status-badge"
      className={`inline-flex items-center text-xs font-semibold px-2.5 py-1 rounded-full ${STATUS_STYLES[status]}`}
    >
      {STATUS_LABELS[status]}
    </span>
  )
}
