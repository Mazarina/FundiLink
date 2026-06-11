import client from '../../api/client'
import type {
  ConsentState,
  ConsentHistoryEntry,
  ConsentType,
  ConsentScope,
  LinkedLearner,
  GuardianView,
} from '../../types'

export async function getConsentState(): Promise<ConsentState> {
  const res = await client.get('/consent/state')
  return res.data
}

export async function getConsentHistory(): Promise<ConsentHistoryEntry[]> {
  const res = await client.get('/consent/history')
  return res.data
}

export async function recordConsent(data: {
  consentType: ConsentType
  scope: ConsentScope
  guardianName: string
  guardianContact: string
}): Promise<{ id: string }> {
  const res = await client.post('/consent', data)
  return res.data
}

export async function revokeConsent(consentType: ConsentType): Promise<void> {
  await client.post('/consent/revoke', { consentType })
}

export async function linkGuardian(data: {
  guardianUserId: string
  guardianName: string
  guardianContact: string
}): Promise<{ id: string }> {
  const res = await client.post('/consent/guardian-links', data)
  return res.data
}

export async function getLinkedLearners(): Promise<LinkedLearner[]> {
  const res = await client.get('/consent/guardian/learners')
  return res.data
}

export async function getGuardianView(learnerId: string): Promise<GuardianView> {
  const res = await client.get(`/consent/guardian/learners/${learnerId}`)
  return res.data
}
