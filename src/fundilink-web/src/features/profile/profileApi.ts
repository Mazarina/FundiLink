import client from '../../api/client'
import type { LearnerProfile } from '../../types'

export async function getMyProfile(): Promise<LearnerProfile> {
  const res = await client.get('/learners/me')
  return res.data
}

export async function updateMyProfile(data: Partial<LearnerProfile>): Promise<void> {
  await client.put('/learners/me', data)
}
