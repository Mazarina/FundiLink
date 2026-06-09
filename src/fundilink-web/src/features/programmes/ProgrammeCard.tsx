import { Link } from 'react-router-dom'

interface ProgrammeCardProps {
  id: string
  name: string
  institutionName: string
  minimumAps: number
  province?: string
  isEligible?: boolean
}

export function ProgrammeCard({ id, name, institutionName, minimumAps, province, isEligible }: ProgrammeCardProps) {
  return (
    <Link
      to={`/programmes/${id}`}
      className="block bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition"
      data-testid="programme-card"
    >
      <div className="flex justify-between items-start gap-3">
        <div className="min-w-0">
          <p className="text-xs text-gray-500 truncate">{institutionName}</p>
          <h3 className="font-semibold text-gray-800">{name}</h3>
          {province && <p className="text-xs text-gray-400 mt-1">{province}</p>}
        </div>
        {isEligible !== undefined && (
          isEligible ? (
            <span
              data-testid="eligible-badge"
              className="shrink-0 inline-flex items-center gap-1 bg-green-100 text-green-700 text-xs font-semibold px-2 py-1 rounded-full"
            >
              ✓ Eligible
            </span>
          ) : (
            <span
              data-testid="ineligible-badge"
              className="shrink-0 inline-flex items-center gap-1 bg-red-100 text-red-700 text-xs font-semibold px-2 py-1 rounded-full"
            >
              ✕ Not yet
            </span>
          )
        )}
      </div>
      <div className="mt-3 text-sm text-gray-600">
        Minimum APS: <span className="font-semibold text-brand-primary">{minimumAps}</span>
      </div>
    </Link>
  )
}
