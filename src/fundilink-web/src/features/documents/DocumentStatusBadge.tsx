import type { DocumentStatus } from '../../types'

const colours: Record<DocumentStatus, string> = {
  Pending: 'bg-gray-100 text-gray-600',
  Verified: 'bg-green-100 text-green-700',
  Rejected: 'bg-red-100 text-red-700',
}

export function DocumentStatusBadge({ status }: { status: DocumentStatus }) {
  return <span className={`text-xs font-medium px-2 py-0.5 rounded-full ${colours[status]}`}>{status}</span>
}
