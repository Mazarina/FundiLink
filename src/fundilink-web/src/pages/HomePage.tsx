import { Link } from 'react-router-dom'

function HomePage() {
  return (
    <main className="min-h-screen bg-brand-light flex flex-col items-center justify-center p-6">
      <div className="text-center max-w-lg">
        <h1 className="text-4xl font-bold text-brand-primary mb-2">FundiLink</h1>
        <p className="text-lg text-gray-600 mb-1">by ZulTek</p>
        <p className="text-xl text-brand-accent font-semibold mb-8">One profile. Every opportunity.</p>
        <p className="text-gray-500 mb-8">
          FundiLink SmartApply helps South African learners find what they qualify for,
          get their documents ready, and track their applications — all in one place.
        </p>
        <div className="flex flex-col sm:flex-row gap-3 justify-center">
          <Link
            to="/register"
            className="bg-brand-primary text-white px-6 py-3 rounded-lg text-sm font-semibold hover:opacity-90"
          >
            Create your profile
          </Link>
          <Link
            to="/login"
            className="border border-brand-primary text-brand-primary px-6 py-3 rounded-lg text-sm font-semibold hover:bg-brand-primary hover:text-white"
          >
            Log in
          </Link>
        </div>
      </div>
    </main>
  )
}

export default HomePage
