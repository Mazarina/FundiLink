import client from '../../api/client'
import type {
  CareerOpportunity,
  CareerMatch,
  CareerInterest,
  CareerOpportunityType,
  OpportunityInterestStatus,
} from '../../types'

export async function getCareerOpportunities(params: {
  fieldOfInterest?: string
  province?: string
  opportunityType?: CareerOpportunityType
}): Promise<CareerOpportunity[]> {
  const res = await client.get('/career', { params })
  return res.data
}

export async function getCareerOpportunity(id: string): Promise<CareerOpportunity> {
  const res = await client.get(`/career/${id}`)
  return res.data
}

export async function getCareerMatches(): Promise<CareerMatch[]> {
  const res = await client.get('/career/matches')
  return res.data
}

export async function getCareerInterests(): Promise<CareerInterest[]> {
  const res = await client.get('/career/interests')
  return res.data
}

export async function trackCareerInterest(data: {
  careerOpportunityId: string
  status: OpportunityInterestStatus
  notes?: string
}): Promise<{ id: string }> {
  const res = await client.post('/career/interests', data)
  return res.data
}

export async function updateCareerInterestStatus(
  id: string,
  data: { newStatus: OpportunityInterestStatus; notes?: string }
): Promise<void> {
  await client.put(`/career/interests/${id}/status`, data)
}
