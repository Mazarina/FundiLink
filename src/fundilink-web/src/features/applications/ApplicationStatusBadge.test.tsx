import { render, screen } from '@testing-library/react'
import { describe, it, expect } from 'vitest'
import { ApplicationStatusBadge } from './ApplicationStatusBadge'
import type { ApplicationStatus } from '../../types'

describe('ApplicationStatusBadge', () => {
  it('renders correct text for each status', () => {
    const cases: Array<[ApplicationStatus, string]> = [
      ['Interested', 'Interested'],
      ['InProgress', 'In Progress'],
      ['Submitted', 'Submitted'],
      ['Accepted', 'Accepted'],
      ['Rejected', 'Rejected'],
      ['Waitlisted', 'Waitlisted'],
    ]
    cases.forEach(([status, label]) => {
      const { unmount } = render(<ApplicationStatusBadge status={status} />)
      expect(screen.getByText(label)).toBeDefined()
      unmount()
    })
  })

  it('Accepted has green styling', () => {
    render(<ApplicationStatusBadge status="Accepted" />)
    expect(screen.getByTestId('status-badge').className).toContain('green')
  })

  it('Rejected has red styling', () => {
    render(<ApplicationStatusBadge status="Rejected" />)
    expect(screen.getByTestId('status-badge').className).toContain('red')
  })
})
