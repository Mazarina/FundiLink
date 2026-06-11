import client from '../../api/client'
import type { DataExport, ErasureRequest } from '../../types'

// Data subject rights (POPIA): export (right of access) and erasure (right to erasure).
// Learner endpoints are owner-scoped; admin endpoints are role-gated server-side.

export async function exportMyData(): Promise<DataExport> {
  const res = await client.get('/data-rights/export')
  return res.data
}

export async function getMyErasureRequests(): Promise<ErasureRequest[]> {
  const res = await client.get('/data-rights/erasure-requests')
  return res.data
}

export async function requestErasure(reason: string | null): Promise<{ id: string }> {
  const res = await client.post('/data-rights/erasure-requests', { reason })
  return res.data
}

export async function getPendingErasureRequests(): Promise<ErasureRequest[]> {
  const res = await client.get('/data-rights/admin/erasure-requests/pending')
  return res.data
}

export async function approveErasureRequest(id: string, note: string | null): Promise<void> {
  await client.post(`/data-rights/admin/erasure-requests/${id}/approve`, { note })
}

export async function rejectErasureRequest(id: string, note: string | null): Promise<void> {
  await client.post(`/data-rights/admin/erasure-requests/${id}/reject`, { note })
}

export async function fulfilErasureRequest(id: string, note: string | null): Promise<void> {
  await client.post(`/data-rights/admin/erasure-requests/${id}/fulfil`, { note })
}
