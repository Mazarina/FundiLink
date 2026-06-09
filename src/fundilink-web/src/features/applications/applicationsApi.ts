import client from '../../api/client'
import type { LearnerApplication, ApplicationStatus } from '../../types'

export async function getApplications(): Promise<LearnerApplication[]> {
  const res = await client.get('/applications')
  return res.data
}

export async function getApplication(id: string): Promise<LearnerApplication> {
  const res = await client.get(`/applications/${id}`)
  return res.data
}

export async function createApplication(data: {
  programmeId: string
  status: ApplicationStatus
  notes?: string
  deadlineDate?: string
}): Promise<{ id: string }> {
  const res = await client.post('/applications', data)
  return res.data
}

export async function updateApplicationStatus(
  id: string,
  data: { newStatus: ApplicationStatus; notes?: string }
): Promise<void> {
  await client.put(`/applications/${id}/status`, data)
}

export async function deleteApplication(id: string): Promise<void> {
  await client.delete(`/applications/${id}`)
}
