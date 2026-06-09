import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { getMyProfile, updateMyProfile } from '../features/profile/profileApi'
import type { LearnerProfile } from '../types'

export default function EditProfilePage() {
  const navigate = useNavigate()
  const [profile, setProfile] = useState<LearnerProfile | null>(null)
  const [error, setError] = useState('')
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    getMyProfile().then(setProfile).catch(() => setError('Could not load profile.'))
  }, [])

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault()
    setSaving(true)
    setError('')
    const form = new FormData(e.currentTarget)
    try {
      await updateMyProfile({
        firstName: form.get('firstName') as string,
        surname: form.get('surname') as string,
        mobileNumber: form.get('mobileNumber') as string,
        province: form.get('province') as string,
        municipality: form.get('municipality') as string,
        suburb: form.get('suburb') as string,
        schoolName: form.get('schoolName') as string,
        schoolProvince: form.get('province') as string,
        gradeLevel: form.get('gradeLevel') as LearnerProfile['gradeLevel'],
        nationality: form.get('nationality') as string,
        gender: form.get('gender') as string || undefined,
        homeLanguage: form.get('homeLanguage') as string || undefined,
      })
      navigate('/profile')
    } catch {
      setError('Failed to save changes. Please try again.')
    } finally {
      setSaving(false)
    }
  }

  if (!profile) return (
    <main className="min-h-screen bg-brand-light flex items-center justify-center">
      <p className="text-gray-500">Loading...</p>
    </main>
  )

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-lg mx-auto">
        <h1 className="text-2xl font-bold text-brand-primary mb-6">Edit Profile</h1>

        {error && <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded text-red-700 text-sm">{error}</div>}

        <form onSubmit={handleSubmit} className="bg-white rounded-xl p-6 shadow-sm space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <Field label="First name" name="firstName" defaultValue={profile.firstName} />
            <Field label="Surname" name="surname" defaultValue={profile.surname} />
          </div>
          <Field label="Mobile number" name="mobileNumber" defaultValue={profile.mobileNumber} />
          <Field label="Nationality" name="nationality" defaultValue={profile.nationality} />
          <Field label="Gender (optional)" name="gender" defaultValue={profile.gender ?? ''} required={false} />
          <Field label="Home language (optional)" name="homeLanguage" defaultValue={profile.homeLanguage ?? ''} required={false} />
          <Field label="Province" name="province" defaultValue={profile.province} />
          <Field label="Municipality" name="municipality" defaultValue={profile.municipality} required={false} />
          <Field label="Suburb" name="suburb" defaultValue={profile.suburb} required={false} />
          <Field label="School name" name="schoolName" defaultValue={profile.schoolName} />

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Grade</label>
            <select name="gradeLevel" defaultValue={profile.gradeLevel}
              className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary">
              <option value="Grade11">Grade 11</option>
              <option value="Grade12">Grade 12</option>
              <option value="PostMatric">Post-Matric</option>
            </select>
          </div>

          <div className="flex gap-3 pt-2">
            <button type="submit" disabled={saving}
              className="flex-1 bg-brand-primary text-white py-2.5 rounded-lg font-semibold text-sm hover:opacity-90 disabled:opacity-50">
              {saving ? 'Saving...' : 'Save changes'}
            </button>
            <button type="button" onClick={() => navigate('/profile')}
              className="flex-1 border border-gray-300 text-gray-700 py-2.5 rounded-lg font-semibold text-sm hover:bg-gray-50">
              Cancel
            </button>
          </div>
        </form>
      </div>
    </main>
  )
}

function Field({ label, name, defaultValue, required = true }: {
  label: string; name: string; defaultValue: string; required?: boolean
}) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1">{label}</label>
      <input name={name} type="text" defaultValue={defaultValue} required={required}
        className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary" />
    </div>
  )
}
