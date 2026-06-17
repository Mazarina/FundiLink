import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { register } from '../features/auth/authApi'
import { AppFooter } from '../features/navigation/AppFooter'

export default function RegisterPage() {
  const navigate = useNavigate()
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault()
    setError('')
    setLoading(true)

    const form = new FormData(e.currentTarget)
    const consentAccepted = form.get('consentAccepted') === 'on'

    if (!consentAccepted) {
      setError('You must accept the privacy notice to register.')
      setLoading(false)
      return
    }

    try {
      await register({
        email: form.get('email') as string,
        password: form.get('password') as string,
        firstName: form.get('firstName') as string,
        surname: form.get('surname') as string,
        dateOfBirth: form.get('dateOfBirth') as string,
        mobileNumber: form.get('mobileNumber') as string,
        province: form.get('province') as string,
        schoolName: form.get('schoolName') as string,
        schoolProvince: form.get('province') as string,
        gradeLevel: form.get('gradeLevel') as 'Grade11' | 'Grade12' | 'PostMatric',
        consentAccepted: true,
      })
      navigate('/login', { state: { message: 'Registration successful. Please log in.' } })
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Registration failed. Please try again.'
      setError(msg)
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="min-h-screen bg-brand-light flex flex-col items-center justify-center p-4">
      <div className="w-full max-w-md bg-white rounded-xl shadow-sm p-8">
        <h1 className="text-2xl font-bold text-brand-primary mb-1">Create your profile</h1>
        <p className="text-gray-500 text-sm mb-6">FundiLink by ZulTek — One profile. Every opportunity.</p>

        {error && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded text-red-700 text-sm">{error}</div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">First name</label>
              <input name="firstName" type="text" required
                className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary" />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Surname</label>
              <input name="surname" type="text" required
                className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary" />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Email address</label>
            <input name="email" type="email" required
              className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary" />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Password</label>
            <input name="password" type="password" required minLength={8}
              className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary" />
            <p className="text-xs text-gray-400 mt-1">Minimum 8 characters, including uppercase, number, and special character.</p>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Date of birth</label>
            <input name="dateOfBirth" type="date" required
              className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary" />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Mobile number</label>
            <input name="mobileNumber" type="tel" required
              className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary" />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Province</label>
            <select name="province" required
              className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary">
              <option value="">Select province...</option>
              {['Eastern Cape','Free State','Gauteng','KwaZulu-Natal','Limpopo','Mpumalanga','North West','Northern Cape','Western Cape'].map(p => (
                <option key={p} value={p}>{p}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">School name</label>
            <input name="schoolName" type="text" required
              className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary" />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Grade</label>
            <select name="gradeLevel" required
              className="w-full border rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-brand-primary">
              <option value="Grade11">Grade 11</option>
              <option value="Grade12">Grade 12</option>
              <option value="PostMatric">Post-Matric</option>
            </select>
          </div>

          {/* POPIA Consent */}
          <div className="bg-blue-50 border border-blue-200 rounded-lg p-4 text-sm text-gray-700">
            <p className="font-semibold text-brand-primary mb-2">Privacy Notice</p>
            <p className="mb-2">
              FundiLink by ZulTek will collect and process your personal information (name, contact details, academic results, and identity information) to help you find and track education and funding opportunities.
            </p>
            <p className="mb-2">
              Your information is protected under the Protection of Personal Information Act (POPIA). You can request access to or deletion of your data at any time.
            </p>
            <p className="text-xs text-gray-500">
              FundiLink is not an official admissions portal for any university, TVET, NSFAS, or government body. It helps you prepare and track your applications.
            </p>
          </div>

          <label className="flex items-start gap-3 cursor-pointer">
            <input name="consentAccepted" type="checkbox" required className="mt-0.5 h-4 w-4 rounded" />
            <span className="text-sm text-gray-700">
              I agree to FundiLink's{' '}
              <Link to="/terms" target="_blank" className="text-brand-primary font-medium hover:underline">Terms of Service</Link>
              {' '}and{' '}
              <Link to="/privacy" target="_blank" className="text-brand-primary font-medium hover:underline">Privacy Policy</Link>
              {' '}and consent to my information being processed as described above.
            </span>
          </label>

          <button type="submit" disabled={loading}
            className="w-full bg-brand-primary text-white py-2.5 rounded-lg font-semibold text-sm hover:opacity-90 transition disabled:opacity-50">
            {loading ? 'Creating profile...' : 'Create free profile'}
          </button>
        </form>

        <p className="text-center text-sm text-gray-500 mt-4">
          Already have an account?{' '}
          <Link to="/login" className="text-brand-primary font-medium hover:underline">Log in</Link>
        </p>
      </div>
      <AppFooter />
    </main>
  )
}
