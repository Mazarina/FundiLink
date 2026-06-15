import { Link } from 'react-router-dom'

export function AppFooter() {
  return (
    <footer className="text-center text-xs text-gray-400 py-6">
      <Link to="/terms" className="hover:underline">Terms of Service</Link>
      <span className="mx-2">·</span>
      <Link to="/privacy" className="hover:underline">Privacy Policy</Link>
    </footer>
  )
}
