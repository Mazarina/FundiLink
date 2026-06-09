import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getMyProfile } from '../features/profile/profileApi'
import type { LearnerProfile } from '../types'
import { useAuth } from '../features/auth/AuthContext'

export default function ProfilePage() {
  const { signOut } = useAuth()
  const [profile, setProfile] = useState<LearnerProfile | null>(null)
  const [error, setError] = useState('')

  useEffect(() => {
    getMyProfile()
      .then(setProfile)
      .catch(() => setError('Could not load profile. Please try again.'))
  }, [])

  if (error) return (
    <main className="min-h-screen bg-brand-light flex items-center justify-center p-4">
      <p className="text-red-600">{error}</p>
    </main>
  )

  if (!profile) return (
    <main className="min-h-screen bg-brand-light flex items-center justify-center p-4">
      <p className="text-gray-500">Loading your profile...</p>
    </main>
  )

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto">
        <header className="flex justify-between items-center mb-6">
          <div>
            <h1 className="text-2xl font-bold text-brand-primary">My Profile</h1>
            <p className="text-gray-500 text-sm">FundiLink by ZulTek</p>
          </div>
          <button onClick={signOut} className="text-sm text-gray-500 hover:text-red-500">Log out</button>
        </header>

        {/* Completeness */}
        <div className="bg-white rounded-xl p-5 mb-4 shadow-sm">
          <div className="flex justify-between items-center mb-2">
            <span className="text-sm font-medium text-gray-700">Profile completeness</span>
            <span className="text-sm font-bold text-brand-primary">{profile.profileCompleteness}%</span>
          </div>
          <div className="w-full bg-gray-200 rounded-full h-2">
            <div className="bg-brand-accent h-2 rounded-full transition-all" style={{ width: `${profile.profileCompleteness}%` }} />
          </div>
        </div>

        {/* Personal info */}
        <div className="bg-white rounded-xl p-5 mb-4 shadow-sm">
          <div className="flex justify-between items-center mb-4">
            <h2 className="font-semibold text-gray-800">Personal Information</h2>
            <Link to="/profile/edit" className="text-sm text-brand-primary hover:underline">Edit</Link>
          </div>
          <dl className="grid grid-cols-2 gap-x-4 gap-y-3 text-sm">
            <InfoRow label="Full name" value={`${profile.firstName} ${profile.surname}`} />
            <InfoRow label="Province" value={profile.province} />
            <InfoRow label="Mobile" value={profile.mobileNumber} />
            <InfoRow label="School" value={profile.schoolName} />
            <InfoRow label="Grade" value={profile.gradeLevel} />
            {profile.isMinor && <InfoRow label="Guardian" value={profile.guardianName ?? 'Not set'} />}
          </dl>
        </div>

        {/* Navigation */}
        <div className="grid grid-cols-2 gap-4">
          <Link to="/academic"
            className="bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition text-center">
            <div className="text-3xl mb-2">📚</div>
            <div className="font-semibold text-gray-800 text-sm">Academic Profile & APS</div>
            <div className="text-xs text-gray-500 mt-1">Enter your results</div>
          </Link>
          <div className="bg-white rounded-xl p-5 shadow-sm opacity-50 text-center">
            <div className="text-3xl mb-2">📄</div>
            <div className="font-semibold text-gray-800 text-sm">Documents</div>
            <div className="text-xs text-gray-500 mt-1">Coming in Phase 3</div>
          </div>
        </div>
      </div>
    </main>
  )
}

function InfoRow({ label, value }: { label: string; value: string }) {
  return (
    <>
      <dt className="text-gray-500">{label}</dt>
      <dd className="font-medium text-gray-800">{value}</dd>
    </>
  )
}
