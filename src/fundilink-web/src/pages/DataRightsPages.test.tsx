import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import DataRightsPage from './DataRightsPage'
import AdminErasureQueuePage from './AdminErasureQueuePage'
import * as api from '../features/data-rights/dataRightsApi'
import type { ErasureRequest } from '../types'

vi.mock('../features/data-rights/dataRightsApi')

const mockedApi = vi.mocked(api)

function renderWithRouter(ui: React.ReactElement) {
  return render(<MemoryRouter>{ui}</MemoryRouter>)
}

const requestedItem: ErasureRequest = {
  id: 'e1',
  learnerId: 'l1',
  status: 'Requested',
  reason: 'No longer needed',
  requestedAt: '2026-06-01T00:00:00Z',
  reviewedAt: null,
  reviewNote: null,
  fulfilledAt: null,
}

describe('DataRightsPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('renders the learner request state', async () => {
    mockedApi.getMyErasureRequests.mockResolvedValue([requestedItem])
    renderWithRouter(<DataRightsPage />)
    expect(await screen.findByText('Requested')).toBeTruthy()
  })

  it('triggers the export API on download', async () => {
    mockedApi.getMyErasureRequests.mockResolvedValue([])
    mockedApi.exportMyData.mockResolvedValue({
      generatedAt: '2026-06-01T00:00:00Z',
      profile: { learnerId: 'l1', firstName: 'Lebo', surname: 'M', province: 'GP', schoolName: 'S', profileCompleteness: 50 },
      applications: [], bursaryApplications: [], documents: [], accommodationInterests: [],
      careerInterests: [], consentHistory: [], disclaimer: 'POPIA',
    })
    // jsdom lacks URL.createObjectURL.
    Object.defineProperty(URL, 'createObjectURL', { value: vi.fn(() => 'blob:x'), writable: true })
    Object.defineProperty(URL, 'revokeObjectURL', { value: vi.fn(), writable: true })

    renderWithRouter(<DataRightsPage />)
    const button = await screen.findByText('Download export')
    fireEvent.click(button)

    await waitFor(() => expect(mockedApi.exportMyData).toHaveBeenCalled())
  })
})

describe('AdminErasureQueuePage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('triggers the fulfil API on the admin action', async () => {
    mockedApi.getPendingErasureRequests.mockResolvedValue([requestedItem])
    mockedApi.fulfilErasureRequest.mockResolvedValue()

    renderWithRouter(<AdminErasureQueuePage />)
    const button = await screen.findByLabelText('Fulfil e1')
    fireEvent.click(button)

    await waitFor(() => expect(mockedApi.fulfilErasureRequest).toHaveBeenCalledWith('e1', null))
  })
})
