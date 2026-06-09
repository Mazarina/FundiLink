import type { SubjectResult } from '../../types'

interface Props {
  apsScore: number
  subjects: SubjectResult[]
}

export function ApsScoreDisplay({ apsScore, subjects }: Props) {
  const eligible = subjects.filter(s => !s.isLifeOrientation)
  const lo = subjects.find(s => s.isLifeOrientation)

  return (
    <div className="bg-white rounded-xl p-6 shadow-sm">
      <div className="text-center mb-6">
        <div className="text-6xl font-bold text-brand-primary mb-1" data-testid="aps-score">{apsScore}</div>
        <div className="text-gray-500 text-sm">Your APS Score</div>
        <div className="mt-2 text-xs text-gray-400">
          Most universities require an APS of 24–36 depending on the programme.
        </div>
      </div>

      <h3 className="text-sm font-semibold text-gray-700 mb-3">Subject Breakdown</h3>
      <div className="space-y-2">
        {eligible.map((s) => (
          <div key={s.subjectName} className="flex justify-between items-center text-sm py-1 border-b border-gray-100 last:border-0">
            <span className="text-gray-700">{s.subjectName}</span>
            <div className="flex items-center gap-3">
              <span className="text-gray-500">{s.percentage}%</span>
              <span className="font-bold text-brand-primary w-4 text-right">{s.apsPoints}</span>
            </div>
          </div>
        ))}
        {lo && (
          <div className="flex justify-between items-center text-sm py-1 text-gray-400">
            <span>{lo.subjectName} <span className="text-xs">(not counted)</span></span>
            <span>{lo.percentage}%</span>
          </div>
        )}
      </div>
    </div>
  )
}
