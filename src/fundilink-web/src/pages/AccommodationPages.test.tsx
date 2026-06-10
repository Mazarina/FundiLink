import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import AccommodationPage from './AccommodationPage'
import AccommodationInterestsPage from './AccommodationInterestsPage'
import * as api from '../features/accommodation/accommodationApi'

vi.mock('../features/accommodation/accommodationApi')

const mockedApi = vi.mocked(api)

function renderWithRouter(ui: React.ReactElement) {
  return render(<MemoryRouter>{ui}</MemoryRouter>)
}

describe('AccommodationPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('renders the guidance-only disclaimer banner', async () => {
    mockedApi.getAccommodationListings.mockResolvedValue([])
    renderWithRouter(<AccommodationPage />)
    expect(await screen.findByText(/guidance only/i)).toBeTruthy()
  })

  it('renders listings from the API', async () => {
    mockedApi.getAccommodationListings.mockResolvedValue([
      {
        id: 'l1',
        name: 'Campus Heights',
        providerName: 'ExampleRes',
        description: 'Example',
        accommodationType: 'ResidenceOnCampus',
        province: 'Gauteng',
        city: 'Johannesburg',
        disclaimer: 'Guidance only',
      },
    ])
    renderWithRouter(<AccommodationPage />)
    expect(await screen.findByText('Campus Heights')).toBeTruthy()
  })

  it('tracks interest via the API', async () => {
    mockedApi.getAccommodationListings.mockResolvedValue([
      {
        id: 'l1',
        name: 'Campus Heights',
        providerName: 'ExampleRes',
        description: 'Example',
        accommodationType: 'Room',
        province: 'Gauteng',
        city: 'Johannesburg',
        disclaimer: 'Guidance only',
      },
    ])
    mockedApi.trackAccommodationInterest.mockResolvedValue({ id: 'i1' })

    renderWithRouter(<AccommodationPage />)
    const button = await screen.findByLabelText('Save interest in Campus Heights')
    fireEvent.click(button)

    await waitFor(() =>
      expect(mockedApi.trackAccommodationInterest).toHaveBeenCalledWith({
        accommodationListingId: 'l1',
        status: 'Saved',
      })
    )
  })
})

describe('AccommodationInterestsPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('updates tracked status via the API', async () => {
    mockedApi.getAccommodationInterests.mockResolvedValue([
      {
        id: 'i1',
        accommodationListingId: 'l1',
        listingName: 'Campus Heights',
        providerName: 'ExampleRes',
        status: 'Saved',
        disclaimer: 'Guidance only',
      },
    ])
    mockedApi.updateAccommodationInterestStatus.mockResolvedValue(undefined)

    renderWithRouter(<AccommodationInterestsPage />)
    const select = (await screen.findByLabelText('Status for Campus Heights')) as HTMLSelectElement
    fireEvent.change(select, { target: { value: 'Contacted' } })

    await waitFor(() =>
      expect(mockedApi.updateAccommodationInterestStatus).toHaveBeenCalledWith('i1', { newStatus: 'Contacted' })
    )
  })
})
