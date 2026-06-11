import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import ConsentPage from './ConsentPage'
import GuardianViewPage from './GuardianViewPage'
import * as api from '../features/consent/consentApi'
import type { ConsentState } from '../types'

vi.mock('../features/consent/consentApi')

const mockedApi = vi.mocked(api)

function renderWithRouter(ui: React.ReactElement) {
  return render(<MemoryRouter>{ui}</MemoryRouter>)
}

const minorState: ConsentState = {
  isMinor: true,
  guardianConsentRequired: true,
  disclaimer: 'Guardian consent is required for minors.',
  consents: [
    { consentType: 'GuardianCoAccess', isGranted: false, scope: null, guardianName: null, recordedAt: null },
  ],
}

describe('ConsentPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('renders consent state with a no-consent badge', async () => {
    mockedApi.getConsentState.mockResolvedValue(minorState)
    renderWithRouter(<ConsentPage />)
    expect(await screen.findByText(/No consent/i)).toBeTruthy()
  })

  it('grants consent via the API', async () => {
    mockedApi.getConsentState.mockResolvedValue(minorState)
    mockedApi.recordConsent.mockResolvedValue({ id: 'c1' })
    renderWithRouter(<ConsentPage />)

    const button = await screen.findByLabelText('Grant GuardianCoAccess')
    fireEvent.click(button)

    await waitFor(() =>
      expect(mockedApi.recordConsent).toHaveBeenCalledWith(
        expect.objectContaining({ consentType: 'GuardianCoAccess', scope: 'ProfileBasic' })
      )
    )
  })
})

describe('GuardianViewPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('renders the minimised guardian view for a learner', async () => {
    mockedApi.getLinkedLearners.mockResolvedValue([
      { learnerId: 'l1', firstName: 'Lebo', surname: 'Mokoena', hasCurrentConsent: true },
    ])
    mockedApi.getGuardianView.mockResolvedValue({
      learnerId: 'l1',
      firstName: 'Lebo',
      surname: 'Mokoena',
      gradeLevel: 'Grade11',
      schoolName: 'Example High',
      province: 'Gauteng',
      profileCompleteness: 60,
      scope: 'ProfileBasic',
      applications: [],
      disclaimer: 'Guidance only',
    })

    renderWithRouter(<GuardianViewPage />)
    const button = await screen.findByLabelText('View Lebo Mokoena')
    fireEvent.click(button)

    await waitFor(() => expect(screen.getByText('Example High')).toBeTruthy())
    expect(mockedApi.getGuardianView).toHaveBeenCalledWith('l1')
  })
})
