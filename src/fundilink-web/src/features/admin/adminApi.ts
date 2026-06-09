import client from '../../api/client'
import type { LearnerSummary, LearnerOverview, AuditLogEntry, PagedResult } from '../../types'

export async function searchLearners(params: { keyword?: string; province?: string; page?: number }): Promise<PagedResult<LearnerSummary>> {
  const res = await client.get('/admin/learners', { params })
  return res.data
}

export async function getLearnerOverview(id: string): Promise<LearnerOverview> {
  const res = await client.get(`/admin/learners/${id}`)
  return res.data
}

export async function verifyDocument(id: string): Promise<void> {
  await client.post(`/admin/documents/${id}/verify`)
}

export async function rejectDocument(id: string, reason: string): Promise<void> {
  await client.post(`/admin/documents/${id}/reject`, { reason })
}

export async function getAuditLog(params: { page?: number; pageSize?: number }): Promise<PagedResult<AuditLogEntry>> {
  const res = await client.get('/audit', { params })
  return res.data
}
