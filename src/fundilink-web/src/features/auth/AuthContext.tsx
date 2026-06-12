import { createContext, useCallback, useContext, useState, type ReactNode } from 'react'
import type { AuthUser } from '../../types'

interface AuthContextValue {
  user: AuthUser | null
  isAuthenticated: boolean
  signIn: (accessToken: string, refreshToken: string, userId: string, email: string) => void
  signOut: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

function decodeRoles(accessToken: string): string[] {
  try {
    const payload = JSON.parse(atob(accessToken.split('.')[1]))
    const raw = payload.role ?? payload[ROLE_CLAIM]
    if (Array.isArray(raw)) return raw
    return raw ? [raw] : []
  } catch {
    return []
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(() => {
    const stored = localStorage.getItem('authUser')
    return stored ? JSON.parse(stored) : null
  })

  const signIn = useCallback((accessToken: string, refreshToken: string, userId: string, email: string) => {
    localStorage.setItem('accessToken', accessToken)
    localStorage.setItem('refreshToken', refreshToken)
    const roles = decodeRoles(accessToken)
    const authUser: AuthUser = { userId, email, role: roles[0], roles }
    localStorage.setItem('authUser', JSON.stringify(authUser))
    setUser(authUser)
  }, [])

  const signOut = useCallback(() => {
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('authUser')
    setUser(null)
  }, [])

  return (
    <AuthContext.Provider value={{ user, isAuthenticated: user !== null, signIn, signOut }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
