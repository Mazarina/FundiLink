import client from '../../api/client'
import type { LearnerDocument, DocumentType } from '../../types'

export async function getMyDocuments(): Promise<LearnerDocument[]> {
  const res = await client.get('/documents')
  return res.data
}

export async function uploadDocument(file: File, documentType: DocumentType): Promise<{ id: string }> {
  const form = new FormData()
  form.append('file', file)
  form.append('documentType', documentType)
  const res = await client.post('/documents', form, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
  return res.data
}

export async function deleteDocument(id: string): Promise<void> {
  await client.delete(`/documents/${id}`)
}

export async function downloadDocument(id: string, fileName: string): Promise<void> {
  const res = await client.get(`/documents/${id}/download`, { responseType: 'blob' })
  const url = URL.createObjectURL(res.data)
  const a = document.createElement('a')
  a.href = url
  a.download = fileName
  a.click()
  URL.revokeObjectURL(url)
}
