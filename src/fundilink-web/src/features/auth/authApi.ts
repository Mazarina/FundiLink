import client from '../../api/client'
import type { AuthTokens, GradeLevel } from '../../types'

export interface RegisterRequest {
  email: string
  password: string
  firstName: string
  surname: string
  dateOfBirth: string
  mobileNumber: string
  province: string
  schoolName: string
  schoolProvince: string
  gradeLevel: GradeLevel
  consentAccepted: boolean
}

export interface LoginRequest {
  email: string
  password: string
}

export async function register(data: RegisterRequest): Promise<{ userId: string; message: string }> {
  const res = await client.post('/auth/register', data)
  return res.data
}

export async function login(data: LoginRequest): Promise<AuthTokens> {
  const res = await client.post('/auth/login', data)
  return res.data
}
