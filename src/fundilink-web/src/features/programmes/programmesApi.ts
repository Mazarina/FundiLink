import client from '../../api/client'
import type { Programme, ProgrammeMatch, PagedResult, InstitutionType } from '../../types'

interface ProgrammeMatchResponse {
  programmeId: string
  programmeName: string
  institutionName: string
  institutionType: InstitutionType
  minimumAps: number
  isEligible: boolean
  apsGap: number
  missingSubjects: string[]
}

export async function searchProgrammes(params: {
  keyword?: string
  type?: string
  province?: string
  page?: number
}): Promise<PagedResult<Programme>> {
  const res = await client.get('/programmes', { params })
  return res.data
}

export async function getProgramme(id: string): Promise<Programme> {
  const res = await client.get(`/programmes/${id}`)
  return res.data
}

export async function getMatches(): Promise<ProgrammeMatch[]> {
  const res = await client.get<ProgrammeMatchResponse[]>('/programmes/matches')
  return res.data.map((m) => ({
    id: m.programmeId,
    name: m.programmeName,
    institutionName: m.institutionName,
    institutionType: m.institutionType,
    province: '',
    minimumAps: m.minimumAps,
    isEligible: m.isEligible,
    apsGap: m.apsGap,
    missingSubjects: m.missingSubjects,
  }))
}
