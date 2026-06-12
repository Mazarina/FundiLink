import { Link, useLocation } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

const STAFF_ROLES = ['Admin', 'SupportAgent', 'SuperAdmin']

export function AppHeader() {
  const { user, signOut } = useAuth()
  const location = useLocation()
  const isStaff = STAFF_ROLES.some((r) => user?.roles?.includes(r))

  const linkClass = (path: string) =>
    `text-sm font-medium ${
      location.pathname === path ? 'text-brand-primary' : 'text-gray-500 hover:text-brand-primary'
    }`

  return (
    <header className="bg-white border-b border-gray-200">
      <div className="max-w-5xl mx-auto px-4 py-3 flex items-center justify-between gap-4">
        <Link to="/dashboard" className="font-bold text-brand-primary whitespace-nowrap">
          FundiLink
        </Link>
        <nav className="flex items-center gap-4 overflow-x-auto">
          <Link to="/dashboard" className={linkClass('/dashboard')}>Dashboard</Link>
          <Link to="/profile" className={linkClass('/profile')}>Profile</Link>
          {isStaff && <Link to="/admin/learners" className={linkClass('/admin/learners')}>Admin</Link>}
          <button onClick={signOut} className="text-sm text-gray-500 hover:text-red-500 whitespace-nowrap">
            Log out
          </button>
        </nav>
      </div>
    </header>
  )
}
