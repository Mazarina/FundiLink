import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import CareerPage from './CareerPage'
import CareerInterestsPage from './CareerInterestsPage'
import * as api from '../features/career/careerApi'

vi.mock('../features/career/careerApi')

const mockedApi = vi.mocked(api)

function renderWithRouter(ui: React.ReactElement) {
  return render(<MemoryRouter>{ui}</MemoryRouter>)
}

describe('CareerPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('renders the guidance-only disclaimer banner', async () => {
    mockedApi.getCareerOpportunities.mockResolvedValue([])
    renderWithRouter(<CareerPage />)
    expect(await screen.findByText(/guidance only/i)).toBeTruthy()
  })

  it('renders opportunities from the API', async () => {
    mockedApi.getCareerOpportunities.mockResolvedValue([
      {
        id: 'o1',
        title: 'IT Learnership',
        providerName: 'ExampleCorp',
        description: 'Example',
        opportunityType: 'Learnership',
        fieldsOfInterest: ['IT'],
        provincesEligible: ['Gauteng'],
        disclaimer: 'Guidance only',
      },
    ])
    renderWithRouter(<CareerPage />)
    expect(await screen.findByText('IT Learnership')).toBeTruthy()
  })

  it('tracks interest via the API', async () => {
    mockedApi.getCareerOpportunities.mockResolvedValue([
      {
        id: 'o1',
        title: 'IT Learnership',
        providerName: 'ExampleCorp',
        description: 'Example',
        opportunityType: 'Learnership',
        fieldsOfInterest: ['IT'],
        provincesEligible: ['Gauteng'],
        disclaimer: 'Guidance only',
      },
    ])
    mockedApi.trackCareerInterest.mockResolvedValue({ id: 'i1' })

    renderWithRouter(<CareerPage />)
    const button = await screen.findByLabelText('Track interest in IT Learnership')
    fireEvent.click(button)

    await waitFor(() =>
      expect(mockedApi.trackCareerInterest).toHaveBeenCalledWith({
        careerOpportunityId: 'o1',
        status: 'Saved',
      })
    )
  })
})

describe('CareerInterestsPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('updates tracked status via the API', async () => {
    mockedApi.getCareerInterests.mockResolvedValue([
      {
        id: 'i1',
        careerOpportunityId: 'o1',
        opportunityTitle: 'IT Learnership',
        providerName: 'ExampleCorp',
        status: 'Saved',
        disclaimer: 'Guidance only',
      },
    ])
    mockedApi.updateCareerInterestStatus.mockResolvedValue(undefined)

    renderWithRouter(<CareerInterestsPage />)
    const select = (await screen.findByLabelText('Status for IT Learnership')) as HTMLSelectElement
    fireEvent.change(select, { target: { value: 'Applied' } })

    await waitFor(() =>
      expect(mockedApi.updateCareerInterestStatus).toHaveBeenCalledWith('i1', { newStatus: 'Applied' })
    )
  })
})
