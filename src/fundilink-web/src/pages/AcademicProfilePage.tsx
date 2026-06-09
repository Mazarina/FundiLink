import { useEffect, useState } from 'react'
import { getAcademicProfile, saveAcademicProfile } from '../features/aps/academicApi'
import { ApsScoreDisplay } from '../features/aps/ApsScoreDisplay'
import { NSC_SUBJECTS } from '../features/aps/nscSubjects'
import type { AcademicProfile, SubjectInput } from '../types'

interface SubjectRow {
  subjectName: string
  percentage: string
  isHomeLanguage: boolean
  isLifeOrientation: boolean
}

export default function AcademicProfilePage() {
  const [profile, setProfile] = useState<AcademicProfile | null>(null)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')

  const [year, setYear] = useState(new Date().getFullYear())
  const [resultType, setResultType] = useState<'Grade12Final' | 'Grade12Prelim' | 'Grade11Prelim'>('Grade12Final')
  const [rows, setRows] = useState<SubjectRow[]>([
    { subjectName: '', percentage: '', isHomeLanguage: false, isLifeOrientation: false },
  ])

  useEffect(() => {
    getAcademicProfile().then((p) => {
      if (!p) return
      setProfile(p)
      setYear(p.year)
      setResultType(p.resultType as typeof resultType)
      setRows(p.subjects.map(s => ({
        subjectName: s.subjectName,
        percentage: String(s.percentage),
        isHomeLanguage: s.isHomeLanguage,
        isLifeOrientation: s.isLifeOrientation,
      })))
    }).catch(() => {})
  }, [])

  function addRow() {
    setRows(r => [...r, { subjectName: '', percentage: '', isHomeLanguage: false, isLifeOrientation: false }])
  }

  function removeRow(idx: number) {
    setRows(r => r.filter((_, i) => i !== idx))
  }

  function updateRow(idx: number, field: keyof SubjectRow, value: string | boolean) {
    setRows(r => r.map((row, i) => i === idx ? { ...row, [field]: value } : row))
  }

  async function handleSave() {
    setError('')
    setSuccess('')

    const subjects: SubjectInput[] = rows
      .filter(r => r.subjectName && r.percentage)
      .map(r => ({
        subjectName: r.subjectName,
        percentage: parseInt(r.percentage, 10),
        isHomeLanguage: r.isHomeLanguage,
        isLifeOrientation: r.subjectName === 'Life Orientation' || r.isLifeOrientation,
      }))

    if (subjects.length === 0) {
      setError('Please add at least one subject.')
      return
    }

    setSaving(true)
    try {
      const result = await saveAcademicProfile({ year, resultType, subjects })
      setSuccess(`APS score calculated: ${result.apsScore}`)
      const updated = await getAcademicProfile()
      if (updated) setProfile(updated)
    } catch {
      setError('Failed to save. Please try again.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto space-y-4">
        <h1 className="text-2xl font-bold text-brand-primary">Academic Profile & APS</h1>

        {profile && <ApsScoreDisplay apsScore={profile.apsScore} subjects={profile.subjects} />}

        <div className="bg-white rounded-xl p-6 shadow-sm">
          <h2 className="font-semibold text-gray-800 mb-4">Enter Your NSC Results</h2>

          {error && <div className="mb-3 p-3 bg-red-50 border border-red-200 rounded text-red-700 text-sm">{error}</div>}
          {success && <div className="mb-3 p-3 bg-green-50 border border-green-200 rounded text-green-700 text-sm">{success}</div>}

          <div className="grid grid-cols-2 gap-4 mb-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Year</label>
              <input type="number" value={year} onChange={e => setYear(Number(e.target.value))} min={2020} max={2030}
                className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary" />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Result type</label>
              <select value={resultType} onChange={e => setResultType(e.target.value as typeof resultType)}
                className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary">
                <option value="Grade12Final">Grade 12 Final</option>
                <option value="Grade12Prelim">Grade 12 Prelim</option>
                <option value="Grade11Prelim">Grade 11 Prelim</option>
              </select>
            </div>
          </div>

          <div className="space-y-3">
            {rows.map((row, idx) => (
              <div key={idx} className="flex gap-2 items-end">
                <div className="flex-1">
                  <label className="block text-xs text-gray-500 mb-1">Subject</label>
                  <select value={row.subjectName}
                    onChange={e => {
                      const val = e.target.value
                      updateRow(idx, 'subjectName', val)
                      if (val === 'Life Orientation') updateRow(idx, 'isLifeOrientation', true)
                      else updateRow(idx, 'isLifeOrientation', false)
                    }}
                    className="w-full border rounded-lg px-2 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary">
                    <option value="">Select subject...</option>
                    {NSC_SUBJECTS.map(s => <option key={s} value={s}>{s}</option>)}
                  </select>
                </div>
                <div className="w-20">
                  <label className="block text-xs text-gray-500 mb-1">%</label>
                  <input type="number" min={0} max={100} value={row.percentage}
                    onChange={e => updateRow(idx, 'percentage', e.target.value)}
                    className="w-full border rounded-lg px-2 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary" />
                </div>
                <div className="flex flex-col items-center pb-1.5">
                  <label className="text-xs text-gray-500 mb-1">HL</label>
                  <input type="checkbox" checked={row.isHomeLanguage}
                    onChange={e => updateRow(idx, 'isHomeLanguage', e.target.checked)}
                    className="h-4 w-4" />
                </div>
                {rows.length > 1 && (
                  <button onClick={() => removeRow(idx)} className="pb-1.5 text-gray-400 hover:text-red-500 text-lg">×</button>
                )}
              </div>
            ))}
          </div>

          <div className="flex gap-3 mt-4">
            <button onClick={addRow} type="button"
              className="text-sm text-brand-primary hover:underline">
              + Add subject
            </button>
            <button onClick={handleSave} disabled={saving}
              className="ml-auto bg-brand-primary text-white px-6 py-2 rounded-lg text-sm font-semibold hover:opacity-90 disabled:opacity-50">
              {saving ? 'Calculating...' : 'Calculate APS'}
            </button>
          </div>

          <p className="text-xs text-gray-400 mt-3">
            Life Orientation is excluded from the APS total. APS uses your top 6 subjects.
          </p>
        </div>
      </div>
    </main>
  )
}
