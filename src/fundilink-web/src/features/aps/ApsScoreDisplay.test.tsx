import { render, screen } from '@testing-library/react'
import { describe, it, expect } from 'vitest'
import { ApsScoreDisplay } from './ApsScoreDisplay'
import type { SubjectResult } from '../../types'

const makeSubject = (overrides: Partial<SubjectResult>): SubjectResult => ({
  subjectName: 'Mathematics',
  percentage: 75,
  apsPoints: 6,
  isHomeLanguage: false,
  isLifeOrientation: false,
  ...overrides,
})

describe('ApsScoreDisplay', () => {
  it('renders the APS score', () => {
    render(<ApsScoreDisplay apsScore={32} subjects={[makeSubject({})]} />)
    expect(screen.getByTestId('aps-score').textContent).toBe('32')
  })

  it('renders all non-LO subjects', () => {
    const subjects = [
      makeSubject({ subjectName: 'Mathematics', percentage: 80, apsPoints: 7 }),
      makeSubject({ subjectName: 'English Home Language', percentage: 65, apsPoints: 5 }),
    ]
    render(<ApsScoreDisplay apsScore={12} subjects={subjects} />)
    expect(screen.getByText('Mathematics')).toBeDefined()
    expect(screen.getByText('English Home Language')).toBeDefined()
  })

  it('shows Life Orientation as not counted', () => {
    const subjects = [
      makeSubject({ subjectName: 'Mathematics', percentage: 70, apsPoints: 6 }),
      makeSubject({ subjectName: 'Life Orientation', percentage: 60, apsPoints: 5, isLifeOrientation: true }),
    ]
    render(<ApsScoreDisplay apsScore={6} subjects={subjects} />)
    expect(screen.getByText(/not counted/i)).toBeDefined()
  })

  it('displays percentages for each subject', () => {
    const subjects = [makeSubject({ subjectName: 'Geography', percentage: 55, apsPoints: 4 })]
    render(<ApsScoreDisplay apsScore={4} subjects={subjects} />)
    expect(screen.getByText('55%')).toBeDefined()
  })

  it('displays APS points for each subject', () => {
    const subjects = [makeSubject({ subjectName: 'History', percentage: 82, apsPoints: 7 })]
    render(<ApsScoreDisplay apsScore={7} subjects={subjects} />)
    const points = screen.getAllByText('7')
    expect(points.length).toBeGreaterThanOrEqual(1)
  })

  it('renders zero APS score correctly', () => {
    render(<ApsScoreDisplay apsScore={0} subjects={[]} />)
    expect(screen.getByTestId('aps-score').textContent).toBe('0')
  })
})
