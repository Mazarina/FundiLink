import { useState } from 'react'
import { Link, useNavigate, useLocation } from 'react-router-dom'
import { login } from '../features/auth/authApi'
import { useAuth } from '../features/auth/AuthContext'

export default function LoginPage() {
  const navigate = useNavigate()
  const location = useLocation()
  const { signIn } = useAuth()
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const message = (location.state as { message?: string })?.message

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault()
    setError('')
    setLoading(true)

    const form = new FormData(e.currentTarget)
    try {
      const tokens = await login({
        email: form.get('email') as string,
        password: form.get('password') as string,
      })
      // Decode userId from token (simple base64 decode of payload)
      const payload = JSON.parse(atob(tokens.accessToken.split('.')[1]))
      signIn(tokens.accessToken, tokens.refreshToken, payload.sub, payload.email)
      navigate('/dashboard')
    } catch {
      setError('Invalid email or password.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="min-h-screen bg-brand-light flex items-center justify-center p-4">
      <div className="w-full max-w-sm bg-white rounded-xl shadow-sm p-8">
        <h1 className="text-2xl font-bold text-brand-primary mb-1">Log in</h1>
        <p className="text-gray-500 text-sm mb-6">FundiLink by ZulTek</p>

        {message && (
          <div className="mb-4 p-3 bg-green-50 border border-green-200 rounded text-green-700 text-sm">{message}</div>
        )}
        {error && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded text-red-700 text-sm">{error}</div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Email address</label>
            <input name="email" type="email" required autoComplete="email"
              className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Password</label>
            <input name="password" type="password" required autoComplete="current-password"
              className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary" />
          </div>

          <button type="submit" disabled={loading}
            className="w-full bg-brand-primary text-white py-2.5 rounded-lg font-semibold text-sm hover:opacity-90 transition disabled:opacity-50">
            {loading ? 'Logging in...' : 'Log in'}
          </button>
        </form>

        <p className="text-center text-sm text-gray-500 mt-4">
          Don't have an account?{' '}
          <Link to="/register" className="text-brand-primary font-medium hover:underline">Create a free profile</Link>
        </p>
      </div>
    </main>
  )
}
