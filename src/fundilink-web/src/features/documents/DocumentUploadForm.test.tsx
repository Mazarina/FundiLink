import { render, screen, fireEvent } from '@testing-library/react'
import { describe, it, expect, vi } from 'vitest'
import { DocumentUploadForm } from './DocumentUploadForm'

const noop = vi.fn()

function makeFile(name: string, type: string, size: number): File {
  const f = new File(['x'.repeat(Math.min(size, 100))], name, { type })
  Object.defineProperty(f, 'size', { value: size })
  return f
}

describe('DocumentUploadForm', () => {
  it('shows error for disallowed file type', () => {
    render(<DocumentUploadForm onUpload={noop} />)
    const input = screen.getByTestId('file-input') as HTMLInputElement
    const file = makeFile('doc.txt', 'text/plain', 1000)
    fireEvent.change(input, { target: { files: [file] } })
    expect(screen.getByTestId('upload-error').textContent).toContain('PDF')
  })
  it('shows error for file over 10 MB', () => {
    render(<DocumentUploadForm onUpload={noop} />)
    const input = screen.getByTestId('file-input') as HTMLInputElement
    const file = makeFile('big.pdf', 'application/pdf', 11 * 1024 * 1024)
    fireEvent.change(input, { target: { files: [file] } })
    expect(screen.getByTestId('upload-error').textContent).toContain('10 MB')
  })
  it('accepts a valid PDF', () => {
    render(<DocumentUploadForm onUpload={noop} />)
    const input = screen.getByTestId('file-input') as HTMLInputElement
    const file = makeFile('cv.pdf', 'application/pdf', 500_000)
    fireEvent.change(input, { target: { files: [file] } })
    expect(screen.queryByTestId('upload-error')).toBeNull()
  })
})
