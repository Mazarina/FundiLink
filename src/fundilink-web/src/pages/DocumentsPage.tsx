import { useEffect, useState } from 'react'
import { getMyDocuments, uploadDocument, deleteDocument, downloadDocument } from '../features/documents/documentsApi'
import { DocumentStatusBadge } from '../features/documents/DocumentStatusBadge'
import { DocumentUploadForm } from '../features/documents/DocumentUploadForm'
import type { LearnerDocument, DocumentType } from '../types'
import { humanizeEnum } from '../utils/format'

export default function DocumentsPage() {
  const [docs, setDocs] = useState<LearnerDocument[]>([])
  const [error, setError] = useState('')

  useEffect(() => { getMyDocuments().then(setDocs).catch(() => setError('Could not load documents.')) }, [])

  async function handleUpload(file: File, docType: DocumentType) {
    await uploadDocument(file, docType)
    const updated = await getMyDocuments()
    setDocs(updated)
  }

  async function handleDelete(id: string) {
    if (!confirm('Are you sure? This will permanently remove this document.')) return
    await deleteDocument(id)
    setDocs(d => d.filter(doc => doc.id !== id))
  }

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto space-y-4">
        <h1 className="text-2xl font-bold text-brand-primary">My Documents</h1>
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-3 text-sm text-blue-700">
          Your documents are stored securely and only accessible to you and authorised FundiLink staff for verification purposes.
        </div>
        {error && <div className="text-red-600 text-sm">{error}</div>}
        <div className="bg-white rounded-xl p-6 shadow-sm">
          <h2 className="font-semibold text-gray-800 mb-4">Upload Document</h2>
          <DocumentUploadForm onUpload={handleUpload} />
        </div>
        <div className="bg-white rounded-xl p-6 shadow-sm">
          <h2 className="font-semibold text-gray-800 mb-4">Your Documents</h2>
          {docs.length === 0 && <p className="text-gray-400 text-sm">No documents uploaded yet.</p>}
          <div className="space-y-3">
            {docs.map(doc => (
              <div key={doc.id} className="flex items-center justify-between border-b border-gray-100 pb-3 last:border-0">
                <div>
                  <div className="text-sm font-medium text-gray-800">{doc.fileName}</div>
                  <div className="text-xs text-gray-500">{humanizeEnum(doc.documentType)}</div>
                  {doc.rejectionReason && <div className="text-xs text-red-600 mt-1">Rejected: {doc.rejectionReason}</div>}
                </div>
                <div className="flex items-center gap-2">
                  <DocumentStatusBadge status={doc.status} />
                  <button onClick={() => downloadDocument(doc.id, doc.fileName)} className="text-xs text-brand-primary hover:underline">Download</button>
                  <button onClick={() => handleDelete(doc.id)} className="text-xs text-red-500 hover:underline">Delete</button>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </main>
  )
}
