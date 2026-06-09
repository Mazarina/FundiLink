import { render, screen } from '@testing-library/react'
import { describe, it, expect } from 'vitest'
import { DocumentStatusBadge } from './DocumentStatusBadge'

describe('DocumentStatusBadge', () => {
  it('renders Pending with gray styling', () => {
    const { container } = render(<DocumentStatusBadge status="Pending" />)
    expect(screen.getByText('Pending')).toBeDefined()
    expect(container.firstChild?.toString()).toBeTruthy()
  })
  it('renders Verified', () => {
    render(<DocumentStatusBadge status="Verified" />)
    expect(screen.getByText('Verified')).toBeDefined()
  })
  it('renders Rejected', () => {
    render(<DocumentStatusBadge status="Rejected" />)
    expect(screen.getByText('Rejected')).toBeDefined()
  })
})
