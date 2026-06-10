import client from '../../api/client'
import type {
  Bursary,
  BursaryMatch,
  BursaryApplication,
  BursaryApplicationStatus,
  BursaryFundingType,
} from '../../types'

export async function getBursaries(params: {
  fieldOfStudy?: string
  province?: string
  fundingType?: BursaryFundingType
}): Promise<Bursary[]> {
  const res = await client.get('/bursaries', { params })
  return res.data
}

export async function getBursary(id: string): Promise<Bursary> {
  const res = await client.get(`/bursaries/${id}`)
  return res.data
}

export async function getBursaryMatches(): Promise<BursaryMatch[]> {
  const res = await client.get('/bursaries/matches')
  return res.data
}

export async function getBursaryApplications(): Promise<BursaryApplication[]> {
  const res = await client.get('/bursary-applications')
  return res.data
}

export async function createBursaryApplication(data: {
  bursaryId: string
  status: BursaryApplicationStatus
  notes?: string
  deadlineDate?: string
}): Promise<{ id: string }> {
  const res = await client.post('/bursary-applications', data)
  return res.data
}

export async function updateBursaryApplicationStatus(
  id: string,
  data: { newStatus: BursaryApplicationStatus; notes?: string }
): Promise<void> {
  await client.put(`/bursary-applications/${id}/status`, data)
}

export async function deleteBursaryApplication(id: string): Promise<void> {
  await client.delete(`/bursary-applications/${id}`)
}
