import client from '../../api/client'
import type { AcademicProfile, SubjectInput, ResultType } from '../../types'

export async function getAcademicProfile(): Promise<AcademicProfile | null> {
  const res = await client.get('/learners/me/academic-profile')
  if (res.status === 204) return null
  return res.data
}

export async function saveAcademicProfile(data: {
  year: number
  resultType: ResultType
  subjects: SubjectInput[]
}): Promise<{ apsScore: number; subjectCount: number }> {
  const res = await client.put('/learners/me/academic-profile', data)
  return res.data
}
