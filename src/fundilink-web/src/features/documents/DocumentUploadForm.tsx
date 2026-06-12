import { useState } from 'react'
import type { DocumentType } from '../../types'
import { humanizeEnum } from '../../utils/format'

const ALLOWED_TYPES = ['application/pdf', 'image/jpeg', 'image/png']
const MAX_SIZE = 10 * 1024 * 1024

interface Props {
  onUpload: (file: File, documentType: DocumentType) => Promise<void>
}

const DOCUMENT_TYPES: DocumentType[] = ['IdDocument', 'MatricCertificate', 'AcademicResults', 'ProofOfResidence', 'GuardianConsent', 'Other']

export function DocumentUploadForm({ onUpload }: Props) {
  const [file, setFile] = useState<File | null>(null)
  const [docType, setDocType] = useState<DocumentType>('IdDocument')
  const [error, setError] = useState('')
  const [uploading, setUploading] = useState(false)

  function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    setError('')
    const f = e.target.files?.[0]
    if (!f) return
    if (!ALLOWED_TYPES.includes(f.type)) {
      setError('Only PDF, JPG, and PNG files are allowed.')
      return
    }
    if (f.size > MAX_SIZE) {
      setError('File must be 10 MB or smaller.')
      return
    }
    setFile(f)
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    if (!file) return
    setUploading(true)
    try {
      await onUpload(file, docType)
      setFile(null)
    } catch {
      setError('Upload failed. Please try again.')
    } finally {
      setUploading(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-3">
      {error && <div className="text-red-600 text-sm" data-testid="upload-error">{error}</div>}
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">Document type</label>
        <select value={docType} onChange={e => setDocType(e.target.value as DocumentType)}
          className="w-full border rounded-lg px-3 py-2 text-sm">
          {DOCUMENT_TYPES.map(t => <option key={t} value={t}>{humanizeEnum(t)}</option>)}
        </select>
      </div>
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">File (PDF, JPG, PNG — max 10 MB)</label>
        <input type="file" accept=".pdf,.jpg,.jpeg,.png" onChange={handleFileChange}
          className="w-full text-sm" data-testid="file-input" />
      </div>
      <button type="submit" disabled={!file || uploading}
        className="bg-brand-primary text-white px-4 py-2 rounded-lg text-sm font-semibold disabled:opacity-50">
        {uploading ? 'Uploading...' : 'Upload'}
      </button>
    </form>
  )
}
