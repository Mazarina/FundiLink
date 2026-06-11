import client from '../../api/client'
import type {
  AuditActivityFilters,
  AuditLogEntry,
  OperationsDashboard,
  PagedResult,
  PopiaOperationsSummary,
} from '../../types'

// Admin reporting (Phase 11). All endpoints are read-only and RBAC-gated server-side.
// Dashboard and POPIA summary return aggregate figures only (no raw PII); the audit
// activity report is a filtered view over the existing append-only audit log (SuperAdmin).

export async function getOperationsDashboard(): Promise<OperationsDashboard> {
  const res = await client.get('/reporting/dashboard')
  return res.data
}

export async function getPopiaOperationsSummary(): Promise<PopiaOperationsSummary> {
  const res = await client.get('/reporting/popia-summary')
  return res.data
}

export async function getAuditActivity(filters: AuditActivityFilters): Promise<PagedResult<AuditLogEntry>> {
  const res = await client.get('/reporting/audit-activity', { params: filters })
  return res.data
}
