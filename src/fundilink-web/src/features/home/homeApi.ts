import client from '../../api/client'
import type { LearnerHomeSummary } from '../../types'

// Owner-scoped learner home summary (Phase 13). Read-only composition of the authenticated
// learner's own data. Guidance only — FundiLink is not an official admissions/funding portal.
export async function getHomeSummary(): Promise<LearnerHomeSummary> {
  const res = await client.get('/home/summary')
  return res.data
}
