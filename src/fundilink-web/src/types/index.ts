export type GradeLevel = 'Grade11' | 'Grade12' | 'PostMatric'
export type ResultType = 'Grade11Prelim' | 'Grade12Prelim' | 'Grade12Final'

export interface AuthTokens {
  accessToken: string
  refreshToken: string
  expiresIn: number
}

export interface AuthUser {
  userId: string
  email: string
}

export interface LearnerProfile {
  id: string
  firstName: string
  surname: string
  dateOfBirth: string
  idNumberMasked?: string
  gender?: string
  homeLanguage?: string
  nationality: string
  mobileNumber: string
  province: string
  municipality: string
  suburb: string
  schoolName: string
  schoolProvince: string
  gradeLevel: GradeLevel
  guardianName?: string
  guardianPhone?: string
  guardianEmail?: string
  isMinor: boolean
  profileCompleteness: number
}

export interface SubjectResult {
  subjectName: string
  subjectCode?: string
  percentage: number
  apsPoints: number
  isHomeLanguage: boolean
  isLifeOrientation: boolean
}

export interface AcademicProfile {
  id: string
  year: number
  resultType: ResultType
  apsScore: number
  apsCalculatedAt?: string
  subjects: SubjectResult[]
}

export interface SubjectInput {
  subjectName: string
  percentage: number
  isHomeLanguage: boolean
  isLifeOrientation: boolean
  subjectCode?: string
}

export type InstitutionType = 'University' | 'TVET' | 'SkillsCentre'
export type ApplicationStatus = 'Interested' | 'InProgress' | 'Submitted' | 'Accepted' | 'Rejected' | 'Waitlisted'

export interface Programme {
  id: string
  name: string
  institutionName: string
  institutionType: InstitutionType
  province: string
  minimumAps: number
  nfqLevel?: number
  applicationOpenDate?: string
  applicationCloseDate?: string
  requiredSubjects?: Array<{ subjectName: string; minimumPercentage: number }>
}

export interface ProgrammeMatch extends Programme {
  isEligible: boolean
  apsGap: number
  missingSubjects: string[]
}

export interface PagedResult<T> {
  items: T[]
  total: number
  page: number
  pageSize: number
}

export interface LearnerApplication {
  id: string
  programmeId: string
  programmeName: string
  institutionName: string
  status: ApplicationStatus
  notes?: string
  deadlineDate?: string
  submittedAt?: string
}
