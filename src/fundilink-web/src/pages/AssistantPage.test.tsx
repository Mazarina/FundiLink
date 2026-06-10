import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import AssistantPage from './AssistantPage'
import * as api from '../features/assistant/assistantApi'

vi.mock('../features/assistant/assistantApi', async (importOriginal) => {
  const actual = await importOriginal<typeof api>()
  return { ...actual, askAssistant: vi.fn() }
})

const mockedAskAssistant = vi.mocked(api.askAssistant)

function renderPage() {
  return render(<MemoryRouter><AssistantPage /></MemoryRouter>)
}

describe('AssistantPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('renders the guidance-only disclaimer', () => {
    renderPage()
    expect(screen.getByText(/automated guidance only/i)).toBeTruthy()
  })

  it('renders a grounded answer with sources from the mocked API', async () => {
    mockedAskAssistant.mockResolvedValue({
      intent: 'WhatIsMyAps',
      answer: 'Hi Thabo. Your estimated APS is 41.',
      sources: ['Your FundiLink academic profile (APS estimate)'],
      guidanceOnly: true,
      disclaimer: 'Verify with the official institution.',
    })

    renderPage()
    fireEvent.click(screen.getByText('What is my APS?'))

    expect(await screen.findByText('Hi Thabo. Your estimated APS is 41.')).toBeTruthy()
    expect(screen.getByText('Your FundiLink academic profile (APS estimate)')).toBeTruthy()
  })

  it('triggers the API with the selected intent', async () => {
    mockedAskAssistant.mockResolvedValue({
      intent: 'WhichBursariesFitMe',
      answer: 'No bursaries match yet.',
      sources: [],
      guidanceOnly: true,
      disclaimer: 'Guidance only.',
    })

    renderPage()
    fireEvent.click(screen.getByText('Which bursaries may fit me?'))

    await waitFor(() =>
      expect(mockedAskAssistant).toHaveBeenCalledWith('WhichBursariesFitMe')
    )
  })
})
