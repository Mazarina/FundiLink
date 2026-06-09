import { Link } from 'react-router-dom'

function NotFoundPage() {
  return (
    <main className="min-h-screen bg-brand-light flex flex-col items-center justify-center p-6">
      <div className="text-center">
        <h1 className="text-6xl font-bold text-brand-primary mb-4">404</h1>
        <p className="text-xl text-gray-600 mb-6">Page not found</p>
        <Link to="/" className="text-brand-primary underline hover:text-brand-accent">
          Go home
        </Link>
      </div>
    </main>
  )
}

export default NotFoundPage
