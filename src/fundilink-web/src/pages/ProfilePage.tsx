import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getMyProfile } from '../features/profile/profileApi'
import type { LearnerProfile } from '../types'
import { useAuth } from '../features/auth/AuthContext'

export default function ProfilePage() {
  const { signOut, user } = useAuth()
  const isStaff = user?.role === 'Admin' || user?.role === 'SupportAgent' || user?.role === 'SuperAdmin'
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
          <Link to="/programmes"
            className="bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition text-center">
            <div className="text-3xl mb-2">🎓</div>
            <div className="font-semibold text-gray-800 text-sm">Browse Programmes</div>
            <div className="text-xs text-gray-500 mt-1">Find courses & institutions</div>
          </Link>
          <Link to="/matches"
            className="bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition text-center">
            <div className="text-3xl mb-2">✨</div>
            <div className="font-semibold text-gray-800 text-sm">My Matches</div>
            <div className="text-xs text-gray-500 mt-1">Programmes you may qualify for</div>
          </Link>
          <Link to="/applications"
            className="bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition text-center">
            <div className="text-3xl mb-2">📋</div>
            <div className="font-semibold text-gray-800 text-sm">My Applications</div>
            <div className="text-xs text-gray-500 mt-1">Track your applications</div>
          </Link>
          <Link to="/documents"
            className="bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition text-center">
            <div className="text-3xl mb-2">📁</div>
            <div className="font-semibold text-gray-800 text-sm">My Documents</div>
            <div className="text-xs text-gray-500 mt-1">Upload & manage documents</div>
          </Link>
          <Link to="/bursaries"
            className="bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition text-center">
            <div className="text-3xl mb-2">💰</div>
            <div className="font-semibold text-gray-800 text-sm">Bursary Hub</div>
            <div className="text-xs text-gray-500 mt-1">Find funding you may qualify for</div>
          </Link>
          <Link to="/accommodation"
            className="bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition text-center">
            <div className="text-3xl mb-2">🏠</div>
            <div className="font-semibold text-gray-800 text-sm">Accommodation</div>
            <div className="text-xs text-gray-500 mt-1">Find places to stay near campus</div>
          </Link>
          <Link to="/career"
            className="bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition text-center">
            <div className="text-3xl mb-2">💼</div>
            <div className="font-semibold text-gray-800 text-sm">Career Opportunities</div>
            <div className="text-xs text-gray-500 mt-1">Learnerships, internships & jobs</div>
          </Link>
          <Link to="/assistant"
            className="bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition text-center">
            <div className="text-3xl mb-2">💬</div>
            <div className="font-semibold text-gray-800 text-sm">Ask FundiLink</div>
            <div className="text-xs text-gray-500 mt-1">Get guidance from your profile</div>
          </Link>
          <Link to="/consent"
            className="bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition text-center">
            <div className="text-3xl mb-2">🛡️</div>
            <div className="font-semibold text-gray-800 text-sm">Guardian Consent</div>
            <div className="text-xs text-gray-500 mt-1">Manage consent &amp; privacy</div>
          </Link>
          <Link to="/guardian"
            className="bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition text-center">
            <div className="text-3xl mb-2">👪</div>
            <div className="font-semibold text-gray-800 text-sm">Guardian Co-Access</div>
            <div className="text-xs text-gray-500 mt-1">View a linked learner</div>
          </Link>
          <Link to="/notifications/preferences"
            className="bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition text-center">
            <div className="text-3xl mb-2">🔔</div>
            <div className="font-semibold text-gray-800 text-sm">Notification Settings</div>
            <div className="text-xs text-gray-500 mt-1">Choose how we reach you</div>
          </Link>
          {isStaff && (
            <Link to="/admin/learners"
              className="bg-white rounded-xl p-5 shadow-sm hover:shadow-md transition text-center">
              <div className="text-3xl mb-2">🛠️</div>
              <div className="font-semibold text-gray-800 text-sm">Admin Portal</div>
              <div className="text-xs text-gray-500 mt-1">Manage learners & documents</div>
            </Link>
          )}
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
