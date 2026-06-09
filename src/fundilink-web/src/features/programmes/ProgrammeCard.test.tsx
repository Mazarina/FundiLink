import { render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { describe, it, expect } from 'vitest'
import { ProgrammeCard } from './ProgrammeCard'

function renderCard(props: Partial<React.ComponentProps<typeof ProgrammeCard>> = {}) {
  return render(
    <MemoryRouter>
      <ProgrammeCard
        id="p1"
        name="BSc Computer Science"
        institutionName="University of Cape Town"
        minimumAps={42}
        {...props}
      />
    </MemoryRouter>
  )
}

describe('ProgrammeCard', () => {
  it('renders programme name and institution name', () => {
    renderCard()
    expect(screen.getByText('BSc Computer Science')).toBeDefined()
    expect(screen.getByText('University of Cape Town')).toBeDefined()
  })

  it('shows green eligible badge when isEligible=true', () => {
    renderCard({ isEligible: true })
    const badge = screen.getByTestId('eligible-badge')
    expect(badge).toBeDefined()
    expect(badge.className).toContain('green')
  })

  it('shows the minimum APS', () => {
    renderCard()
    expect(screen.getByText('42')).toBeDefined()
  })
})
