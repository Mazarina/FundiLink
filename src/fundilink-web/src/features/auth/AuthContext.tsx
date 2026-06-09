import { createContext, useCallback, useContext, useState, type ReactNode } from 'react'
import type { AuthUser } from '../../types'

interface AuthContextValue {
  user: AuthUser | null
  isAuthenticated: boolean
  signIn: (accessToken: string, refreshToken: string, userId: string, email: string) => void
  signOut: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(() => {
    const stored = localStorage.getItem('authUser')
    return stored ? JSON.parse(stored) : null
  })

  const signIn = useCallback((accessToken: string, refreshToken: string, userId: string, email: string) => {
    localStorage.setItem('accessToken', accessToken)
    localStorage.setItem('refreshToken', refreshToken)
    const authUser: AuthUser = { userId, email }
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
