import { render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import HomeDashboardPage from './HomeDashboardPage'
import * as api from '../features/home/homeApi'
import type { LearnerHomeSummary } from '../types'

vi.mock('../features/home/homeApi')

const mockedApi = vi.mocked(api)

function renderPage() {
  render(
    <MemoryRouter>
      <HomeDashboardPage />
    </MemoryRouter>,
  )
}

const fullSummary: LearnerHomeSummary = {
  firstName: 'Thabo',
  profileCompleteness: 60,
  programmeApplicationCounts: [{ status: 'Submitted', count: 2 }],
  programmeApplicationTotal: 2,
  bursaryApplicationCounts: [{ status: 'Researching', count: 1 }],
  bursaryApplicationTotal: 1,
  pendingDocumentCount: 3,
  upcomingDeadlines: [
    { kind: 'ProgrammeApplication', opportunityName: 'BSc Engineering', deadlineDate: '2026-07-01T00:00:00Z' },
  ],
  recentNotifications: [
    { id: 'n1', notificationType: 'DeadlineReminder', channel: 'Email', status: 'Sent', sentAt: '2026-06-10T09:00:00Z' },
  ],
}

const emptySummary: LearnerHomeSummary = {
  firstName: 'Lerato',
  profileCompleteness: 20,
  programmeApplicationCounts: [],
  programmeApplicationTotal: 0,
  bursaryApplicationCounts: [],
  bursaryApplicationTotal: 0,
  pendingDocumentCount: 0,
  upcomingDeadlines: [],
  recentNotifications: [],
}

describe('HomeDashboardPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('renders summary cards from the loaded summary', async () => {
    mockedApi.getHomeSummary.mockResolvedValue(fullSummary)

    renderPage()

    expect(await screen.findByText('Welcome back, Thabo')).toBeTruthy()
    expect(screen.getByText('60%')).toBeTruthy()
    expect(screen.getByText('BSc Engineering')).toBeTruthy()
    expect(screen.getByText('Programme applications')).toBeTruthy()
  })

  it('renders empty states when there is no activity', async () => {
    mockedApi.getHomeSummary.mockResolvedValue(emptySummary)

    renderPage()

    expect(await screen.findByText('Welcome back, Lerato')).toBeTruthy()
    expect(screen.getByText('No programme applications yet.')).toBeTruthy()
    expect(screen.getByText('No deadlines in the next 30 days.')).toBeTruthy()
    expect(screen.getByText('No recent notifications.')).toBeTruthy()
  })

  it('links each card to its feature route', async () => {
    mockedApi.getHomeSummary.mockResolvedValue(fullSummary)

    renderPage()

    await screen.findByText('Welcome back, Thabo')
    const hrefs = screen.getAllByRole('link').map((a) => a.getAttribute('href'))
    expect(hrefs).toContain('/applications')
    expect(hrefs).toContain('/bursary-applications')
    expect(hrefs).toContain('/documents')
    expect(hrefs).toContain('/notifications/history')
    expect(hrefs).toContain('/profile')
  })
})
