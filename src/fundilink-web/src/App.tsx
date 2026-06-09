import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { AuthProvider } from './features/auth/AuthContext'
import { ProtectedRoute } from './features/auth/ProtectedRoute'
import HomePage from './pages/HomePage'
import NotFoundPage from './pages/NotFoundPage'
import RegisterPage from './pages/RegisterPage'
import LoginPage from './pages/LoginPage'
import ProfilePage from './pages/ProfilePage'
import EditProfilePage from './pages/EditProfilePage'
import AcademicProfilePage from './pages/AcademicProfilePage'
import ProgrammesPage from './pages/ProgrammesPage'
import ProgrammeDetailPage from './pages/ProgrammeDetailPage'
import MatchesPage from './pages/MatchesPage'
import ApplicationsPage from './pages/ApplicationsPage'
import ApplicationDetailPage from './pages/ApplicationDetailPage'

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/profile" element={<ProtectedRoute><ProfilePage /></ProtectedRoute>} />
          <Route path="/profile/edit" element={<ProtectedRoute><EditProfilePage /></ProtectedRoute>} />
          <Route path="/academic" element={<ProtectedRoute><AcademicProfilePage /></ProtectedRoute>} />
          <Route path="/programmes" element={<ProtectedRoute><ProgrammesPage /></ProtectedRoute>} />
          <Route path="/programmes/:id" element={<ProtectedRoute><ProgrammeDetailPage /></ProtectedRoute>} />
          <Route path="/matches" element={<ProtectedRoute><MatchesPage /></ProtectedRoute>} />
          <Route path="/applications" element={<ProtectedRoute><ApplicationsPage /></ProtectedRoute>} />
          <Route path="/applications/:id" element={<ProtectedRoute><ApplicationDetailPage /></ProtectedRoute>} />
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  )
}

export default App
