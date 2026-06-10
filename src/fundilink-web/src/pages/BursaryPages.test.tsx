import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import BursaryMatchesPage from './BursaryMatchesPage'
import BursaryApplicationsPage from './BursaryApplicationsPage'
import * as api from '../features/bursaries/bursariesApi'

vi.mock('../features/bursaries/bursariesApi')

const mockedApi = vi.mocked(api)

function renderWithRouter(ui: React.ReactElement) {
  return render(<MemoryRouter>{ui}</MemoryRouter>)
}

describe('BursaryMatchesPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('renders matches with guidance-only badge', async () => {
    mockedApi.getBursaryMatches.mockResolvedValue([
      {
        bursaryId: 'b1',
        name: 'Funza Lushaka (guidance)',
        providerName: 'DBE',
        fundingType: 'FullCost',
        minimumAps: 26,
        reasons: ['Your APS (40) meets the minimum of 26.'],
        guidanceOnly: true,
        disclaimer: 'Guidance only',
      },
    ])

    renderWithRouter(<BursaryMatchesPage />)

    expect(await screen.findByText('Funza Lushaka (guidance)')).toBeTruthy()
    expect(screen.getByText('You may qualify')).toBeTruthy()
  })

  it('renders the guidance-only disclaimer banner', async () => {
    mockedApi.getBursaryMatches.mockResolvedValue([])
    renderWithRouter(<BursaryMatchesPage />)
    expect(await screen.findByText(/guidance only/i)).toBeTruthy()
  })
})

describe('BursaryApplicationsPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('updates tracker status via the API', async () => {
    mockedApi.getBursaryApplications.mockResolvedValue([
      {
        id: 'a1',
        bursaryId: 'b1',
        bursaryName: 'Test Bursary',
        providerName: 'Provider',
        status: 'Researching',
        disclaimer: 'Guidance only',
      },
    ])
    mockedApi.updateBursaryApplicationStatus.mockResolvedValue(undefined)

    renderWithRouter(<BursaryApplicationsPage />)

    const select = (await screen.findByLabelText('Status for Test Bursary')) as HTMLSelectElement
    fireEvent.change(select, { target: { value: 'Submitted' } })

    await waitFor(() =>
      expect(mockedApi.updateBursaryApplicationStatus).toHaveBeenCalledWith('a1', { newStatus: 'Submitted' })
    )
  })
})
