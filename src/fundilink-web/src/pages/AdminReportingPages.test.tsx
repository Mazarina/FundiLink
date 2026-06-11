import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import AdminReportingDashboardPage from './AdminReportingDashboardPage'
import AdminAuditActivityPage from './AdminAuditActivityPage'
import * as api from '../features/reporting/reportingApi'
import * as notificationsApi from '../features/notifications/notificationsApi'
import type { OperationsDashboard, PopiaOperationsSummary } from '../types'

vi.mock('../features/reporting/reportingApi')
vi.mock('../features/notifications/notificationsApi')

const mockedApi = vi.mocked(api)
const mockedNotificationsApi = vi.mocked(notificationsApi)

function renderWithRouter(ui: React.ReactElement) {
  return render(<MemoryRouter>{ui}</MemoryRouter>)
}

const dashboard: OperationsDashboard = {
  totalLearners: 42,
  learnersByProvince: [{ category: 'Gauteng', count: 30 }, { category: 'Limpopo', count: 12 }],
  applicationsByStatus: [{ category: 'Submitted', count: 10 }],
  bursaryApplicationsByStatus: [{ category: 'Awarded', count: 3 }],
  documentsByStatus: [{ category: 'Pending', count: 5 }],
  pendingDocumentVerifications: 5,
  pendingErasureRequests: 2,
  consentGrants: 8,
  consentRevocations: 1,
}

const popia: PopiaOperationsSummary = {
  pendingDocumentVerifications: 5,
  pendingErasureRequests: 2,
}

describe('AdminReportingDashboardPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('renders aggregate cards', async () => {
    mockedApi.getOperationsDashboard.mockResolvedValue(dashboard)
    mockedApi.getPopiaOperationsSummary.mockResolvedValue(popia)

    renderWithRouter(<AdminReportingDashboardPage />)

    expect(await screen.findByText('42')).toBeTruthy()
    expect(screen.getByText('Total learners')).toBeTruthy()
    expect(screen.getByText('Gauteng')).toBeTruthy()
  })

  it('renders the POPIA operations summary with pending counts and links', async () => {
    mockedApi.getOperationsDashboard.mockResolvedValue(dashboard)
    mockedApi.getPopiaOperationsSummary.mockResolvedValue(popia)

    renderWithRouter(<AdminReportingDashboardPage />)

    await screen.findByText('POPIA operations')
    expect(screen.getByText('Pending document verifications')).toBeTruthy()
    expect(screen.getByText('Pending erasure requests')).toBeTruthy()
    const erasureLink = screen.getByText('Pending erasure requests').closest('a')
    expect(erasureLink?.getAttribute('href')).toBe('/admin/erasure-requests')
  })

  it('runs the deadline reminder pass via the API and shows the result', async () => {
    mockedApi.getOperationsDashboard.mockResolvedValue(dashboard)
    mockedApi.getPopiaOperationsSummary.mockResolvedValue(popia)
    mockedNotificationsApi.runDeadlineReminders.mockResolvedValue({
      learnersWithUpcomingDeadlines: 5,
      remindersSent: 4,
      remindersSkippedAlreadySent: 1,
    })

    renderWithRouter(<AdminReportingDashboardPage />)

    const button = await screen.findByText('Run deadline reminders')
    fireEvent.click(button)

    await waitFor(() => expect(mockedNotificationsApi.runDeadlineReminders).toHaveBeenCalledWith(14))
    expect(await screen.findByText(/4 sent, 1 skipped/)).toBeTruthy()
  })
})

describe('AdminAuditActivityPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('triggers the audit API when filters are applied', async () => {
    mockedApi.getAuditActivity.mockResolvedValue({ items: [], total: 0, page: 1, pageSize: 50 })

    renderWithRouter(<AdminAuditActivityPage />)

    await waitFor(() => expect(mockedApi.getAuditActivity).toHaveBeenCalledTimes(1))

    fireEvent.change(screen.getByLabelText('Filter by action'), { target: { value: 'SearchLearners' } })
    fireEvent.click(screen.getByText('Apply filters'))

    await waitFor(() =>
      expect(mockedApi.getAuditActivity).toHaveBeenLastCalledWith(
        expect.objectContaining({ action: 'SearchLearners' }),
      ),
    )
  })
})
