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
