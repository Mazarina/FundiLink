# Phase 11 — Admin Reporting & POPIA Operations Dashboard (MVP)

You are building Phase 11 of FundiLink by ZulTek on branch `claude/happy-dirac-n7qgtg`.

FundiLink is a South African student opportunity platform (Clean Architecture: ASP.NET Core 8 backend, React+Vite+TypeScript frontend). Phases 1-10 are complete and green (all .NET + frontend tests pass, both build clean).

**FIRST**: Read `/home/user/FundiLink/CLAUDE.md` fully. Key rules:
- NEVER commit secrets, API keys, or credentials. Any third-party key MUST come from environment variables / user-secrets only — placeholder `SET_VIA_ENV` in `appsettings.json`.
- dotnet is at `/usr/local/dotnet/dotnet`.
- Push to `claude/happy-dirac-n7qgtg` only (this worktree may be on its own local branch — push with `git push origin HEAD:claude/happy-dirac-n7qgtg`).
- All existing tests must keep passing.
- Follow existing CQRS/MediatR patterns (`Features/Admin/`, `Features/DataRights/`, `Features/Consent/`), the typed-DTO pattern, and the append-only POPIA-minimal audit-log pattern (`AuditLogEntry`).

**CRITICAL POSITIONING / SAFETY (POPIA):**
- This phase is **read-only reporting** for staff. It must NOT expose raw personal information beyond what existing RBAC already permits, and must NOT introduce any new way to read learner sensitive fields. Aggregate/derived figures only where possible.
- Reporting reads append-only audit data and aggregate counts; it never mutates data. Any staff access to a learner-identifying report row must itself be RBAC-gated and (where it surfaces an individual learner) audit-logged, consistent with existing admin patterns.
- No third-party analytics/telemetry provider integration in this phase — all aggregation is computed in-process behind interfaces; a real provider may be wired later behind the same interface (key via env only).

## Scope — MVP only
1. **Operations dashboard (aggregates)** — counts and simple time-bucketed series staff can act on: total learners, learners by province, applications by status, bursary applications by status, documents by verification status, pending document-verification count, pending erasure-request count, consent grants vs revocations. Aggregates only (no raw PII in the aggregate endpoints).
2. **Audit activity report** — a filtered, paged view over the existing append-only `AuditLogEntry` data (by action, actor role, date range) for SuperAdmin, building on the existing `GetAuditLog` query.
3. **POPIA operations summary** — surface the open POPIA work queues already implemented (pending document verifications, pending erasure requests) as a single staff overview, linking to the existing queues.

### 1. Domain / Application
- No new persisted entity is required if aggregates can be computed from existing data. If a read-model/projection is genuinely needed, justify it and keep it derived (no new PII).
- `Features/Reporting/` CQRS queries: dashboard aggregates, audit activity report (extend/compose existing audit query), POPIA operations summary. Typed DTOs only — never `object`/`dynamic`.
- Aggregation behind an interface (`IReportingService` or query handlers) — deterministic, in-process; no external analytics provider.

### 2. API
- `ReportingController` under `api/v1/reporting` (or extend `AdminController`), RBAC-gated (dashboard: SupportAgent/Admin/SuperAdmin; audit report: SuperAdmin). `[Authorize]`, validate inputs at boundary. Reads only — no mutations. Audit-log any access that surfaces an individual learner (consistent with existing admin patterns).

### 3. Frontend
- `src/features/reporting/` (api wrappers), an admin dashboard page (cards for the aggregates + the POPIA operations summary linking to existing queues), and an audit activity report page (filters + paged table) for SuperAdmin. Routes in `App.tsx` (role-gated) + an admin profile tile.

### 4. Tests
- Backend: >= 4 tests (dashboard aggregates compute expected counts from seeded/mocked data; audit report filters by action/role/date; RBAC — non-staff rejected; POPIA summary returns pending counts).
- Frontend: >= 3 tests (dashboard renders aggregate cards, audit report filter triggers API, POPIA summary renders pending counts / links).

### 5. Execution steps
1. Read CLAUDE.md, then `Features/Admin/` (especially `GetAuditLog`, `SearchLearners`) and `Features/DataRights/` as templates.
2. Domain/Application -> Infrastructure (impl + DI if needed) -> API -> Frontend -> Tests.
3. `/usr/local/dotnet/dotnet build FundiLink.sln` and `dotnet test` — all green.
4. Frontend `npm run build` and `npm test -- --run` — clean.
5. Update `ROADMAP.md` Phase 11 checklist, `docs/15-api-specification.md`, and `docs/07-privacy-compliance.md` (note that reporting is read-only, aggregate-first, RBAC-gated, and adds no new PII exposure).
6. Write the Phase 12 prompt into `prompts/NEXT_PROMPT.md`.
7. Commit: `Add Phase 11 admin reporting and POPIA operations dashboard`.
8. Push: `git push origin HEAD:claude/happy-dirac-n7qgtg`.

**Security/privacy notes for commit message:**
- Reporting is read-only and aggregate-first; RBAC-gated; adds no new way to read learner sensitive fields.
- Any access that surfaces an individual learner is append-only audit-logged, consistent with existing admin patterns.
- No third-party analytics/telemetry integration — deterministic in-process aggregation behind interfaces; any future key via env only.
- No secrets committed.
