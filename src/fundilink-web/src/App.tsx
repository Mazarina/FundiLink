import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { AuthProvider } from './features/auth/AuthContext'
import { ProtectedRoute } from './features/auth/ProtectedRoute'
import HomePage from './pages/HomePage'
import NotFoundPage from './pages/NotFoundPage'
import RegisterPage from './pages/RegisterPage'
import LoginPage from './pages/LoginPage'
import HomeDashboardPage from './pages/HomeDashboardPage'
import ProfilePage from './pages/ProfilePage'
import EditProfilePage from './pages/EditProfilePage'
import AcademicProfilePage from './pages/AcademicProfilePage'
import ProgrammesPage from './pages/ProgrammesPage'
import ProgrammeDetailPage from './pages/ProgrammeDetailPage'
import MatchesPage from './pages/MatchesPage'
import ApplicationsPage from './pages/ApplicationsPage'
import ApplicationDetailPage from './pages/ApplicationDetailPage'
import DocumentsPage from './pages/DocumentsPage'
import AdminLearnersPage from './pages/AdminLearnersPage'
import AdminLearnerDetailPage from './pages/AdminLearnerDetailPage'
import NotificationPreferencesPage from './pages/NotificationPreferencesPage'
import NotificationHistoryPage from './pages/NotificationHistoryPage'
import AuditLogPage from './pages/AuditLogPage'
import BursariesPage from './pages/BursariesPage'
import BursaryDetailPage from './pages/BursaryDetailPage'
import BursaryMatchesPage from './pages/BursaryMatchesPage'
import BursaryApplicationsPage from './pages/BursaryApplicationsPage'
import AssistantPage from './pages/AssistantPage'
import AccommodationPage from './pages/AccommodationPage'
import AccommodationInterestsPage from './pages/AccommodationInterestsPage'
import CareerPage from './pages/CareerPage'
import CareerInterestsPage from './pages/CareerInterestsPage'
import ConsentPage from './pages/ConsentPage'
import GuardianViewPage from './pages/GuardianViewPage'
import DataRightsPage from './pages/DataRightsPage'
import AdminErasureQueuePage from './pages/AdminErasureQueuePage'
import AdminReportingDashboardPage from './pages/AdminReportingDashboardPage'
import AdminAuditActivityPage from './pages/AdminAuditActivityPage'

const ADMIN_ROLES = ['Admin', 'SupportAgent', 'SuperAdmin']

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/dashboard" element={<ProtectedRoute><HomeDashboardPage /></ProtectedRoute>} />
          <Route path="/profile" element={<ProtectedRoute><ProfilePage /></ProtectedRoute>} />
          <Route path="/profile/edit" element={<ProtectedRoute><EditProfilePage /></ProtectedRoute>} />
          <Route path="/academic" element={<ProtectedRoute><AcademicProfilePage /></ProtectedRoute>} />
          <Route path="/programmes" element={<ProtectedRoute><ProgrammesPage /></ProtectedRoute>} />
          <Route path="/programmes/:id" element={<ProtectedRoute><ProgrammeDetailPage /></ProtectedRoute>} />
          <Route path="/matches" element={<ProtectedRoute><MatchesPage /></ProtectedRoute>} />
          <Route path="/applications" element={<ProtectedRoute><ApplicationsPage /></ProtectedRoute>} />
          <Route path="/applications/:id" element={<ProtectedRoute><ApplicationDetailPage /></ProtectedRoute>} />
          <Route path="/documents" element={<ProtectedRoute><DocumentsPage /></ProtectedRoute>} />
          <Route path="/bursaries" element={<ProtectedRoute><BursariesPage /></ProtectedRoute>} />
          <Route path="/bursaries/matches" element={<ProtectedRoute><BursaryMatchesPage /></ProtectedRoute>} />
          <Route path="/bursaries/:id" element={<ProtectedRoute><BursaryDetailPage /></ProtectedRoute>} />
          <Route path="/bursary-applications" element={<ProtectedRoute><BursaryApplicationsPage /></ProtectedRoute>} />
          <Route path="/assistant" element={<ProtectedRoute><AssistantPage /></ProtectedRoute>} />
          <Route path="/accommodation" element={<ProtectedRoute><AccommodationPage /></ProtectedRoute>} />
          <Route path="/accommodation/interests" element={<ProtectedRoute><AccommodationInterestsPage /></ProtectedRoute>} />
          <Route path="/career" element={<ProtectedRoute><CareerPage /></ProtectedRoute>} />
          <Route path="/career/interests" element={<ProtectedRoute><CareerInterestsPage /></ProtectedRoute>} />
          <Route path="/consent" element={<ProtectedRoute><ConsentPage /></ProtectedRoute>} />
          <Route path="/guardian" element={<ProtectedRoute><GuardianViewPage /></ProtectedRoute>} />
          <Route path="/data-rights" element={<ProtectedRoute><DataRightsPage /></ProtectedRoute>} />
          <Route path="/notifications/preferences" element={<ProtectedRoute><NotificationPreferencesPage /></ProtectedRoute>} />
          <Route path="/notifications/history" element={<ProtectedRoute><NotificationHistoryPage /></ProtectedRoute>} />
          <Route path="/admin/learners" element={<ProtectedRoute roles={ADMIN_ROLES}><AdminLearnersPage /></ProtectedRoute>} />
          <Route path="/admin/learners/:id" element={<ProtectedRoute roles={ADMIN_ROLES}><AdminLearnerDetailPage /></ProtectedRoute>} />
          <Route path="/admin/audit" element={<ProtectedRoute roles={['SuperAdmin']}><AuditLogPage /></ProtectedRoute>} />
          <Route path="/admin/erasure-requests" element={<ProtectedRoute roles={ADMIN_ROLES}><AdminErasureQueuePage /></ProtectedRoute>} />
          <Route path="/admin/reporting" element={<ProtectedRoute roles={ADMIN_ROLES}><AdminReportingDashboardPage /></ProtectedRoute>} />
          <Route path="/admin/audit-activity" element={<ProtectedRoute roles={['SuperAdmin']}><AdminAuditActivityPage /></ProtectedRoute>} />
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  )
}

export default App
