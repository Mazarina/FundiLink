import client from '../../api/client'
import type {
  AccommodationListing,
  AccommodationMatch,
  AccommodationInterest,
  AccommodationType,
  OpportunityInterestStatus,
} from '../../types'

export async function getAccommodationListings(params: {
  province?: string
  nearInstitution?: string
  accommodationType?: AccommodationType
}): Promise<AccommodationListing[]> {
  const res = await client.get('/accommodation', { params })
  return res.data
}

export async function getAccommodationListing(id: string): Promise<AccommodationListing> {
  const res = await client.get(`/accommodation/${id}`)
  return res.data
}

export async function getAccommodationMatches(): Promise<AccommodationMatch[]> {
  const res = await client.get('/accommodation/matches')
  return res.data
}

export async function getAccommodationInterests(): Promise<AccommodationInterest[]> {
  const res = await client.get('/accommodation/interests')
  return res.data
}

export async function trackAccommodationInterest(data: {
  accommodationListingId: string
  status: OpportunityInterestStatus
  notes?: string
}): Promise<{ id: string }> {
  const res = await client.post('/accommodation/interests', data)
  return res.data
}

export async function updateAccommodationInterestStatus(
  id: string,
  data: { newStatus: OpportunityInterestStatus; notes?: string }
): Promise<void> {
  await client.put(`/accommodation/interests/${id}/status`, data)
}
