# Phase 10 — Data Subject Rights: Export & Erasure (POPIA, MVP)

You are building Phase 10 of FundiLink by ZulTek on branch `claude/happy-dirac-n7qgtg`.

FundiLink is a South African student opportunity platform (Clean Architecture: ASP.NET Core 8 backend, React+Vite+TypeScript frontend). Phases 1-9 are complete and green (all .NET + frontend tests pass, both build clean).

**FIRST**: Read `/home/user/FundiLink/CLAUDE.md` fully. Key rules:
- NEVER commit secrets, API keys, or credentials. Any third-party key MUST come from environment variables / user-secrets only — placeholder `SET_VIA_ENV` in `appsettings.json`.
- dotnet is at `/usr/local/dotnet/dotnet`.
- Push to `claude/happy-dirac-n7qgtg` only (this worktree may be on its own local branch — push with `git push origin HEAD:claude/happy-dirac-n7qgtg`).
- All existing tests must keep passing.
- Follow existing CQRS/MediatR patterns (`Features/Consent/`, `Features/Bursaries/`, `Features/Admin/`), the stub-behind-interface pattern, the disclaimer pattern, and the append-only POPIA-minimal audit-log pattern (`AuditLogEntry`, `GuardianConsent`).

**CRITICAL POSITIONING / SAFETY (POPIA):**
- POPIA grants data subjects the right of access (export their data) and the right to erasure (deletion). This phase implements learner-initiated **data export** and **erasure requests**, with admin review/fulfilment.
- Erasure must be careful and reversible-until-confirmed: a learner raises a request; deletion is a deliberate, audited admin action. NEVER hard-delete audit logs, consent history, or other append-only legal records — POPIA requires retaining proof of lawful processing/consent. Erasure soft-deletes/anonymises learner profile and personal data while preserving append-only audit/consent records (which are already POPIA-minimal and do not contain sensitive payloads).
- All export generation and all erasure request / approval / fulfilment actions are append-only audit-logged.
- No third-party storage/email integration in this phase — the export is generated in-process (typed DTO / JSON) behind an interface with a deterministic implementation; a real delivery channel may be wired later behind the same interface (key via env only).

## Scope — MVP only
1. **Data export (right of access)** — a learner can request and download a structured export of their own FundiLink data (profile, academic profile, applications, bursary applications, documents metadata, interests, consent history). Owner-scoped, typed DTO.
2. **Erasure request (right to erasure)** — a learner can raise an erasure request (status: Requested, Approved, Rejected, Fulfilled). An admin reviews and fulfils it; fulfilment soft-deletes/anonymises personal data while preserving append-only audit/consent records.
3. **Surface request state** — show the learner the status of their export/erasure requests; show admins a queue of pending requests.

### 1. Domain / Application
- New entity `ErasureRequest` (audited status transitions); EF config, DbSet, migration.
- `Features/DataRights/` CQRS: request export (returns typed export DTO), request erasure, query my requests; admin: list pending, approve/reject, fulfil erasure. Typed DTOs only.
- Erasure fulfilment service behind an interface — deterministic, anonymises/soft-deletes learner personal data; NEVER touches append-only audit/consent logs.

### 2. API
- `DataRightsController` under `api/v1/...` (learner, owner-scoped) and admin endpoints (RBAC: SupportAgent/Admin/SuperAdmin). `[Authorize]`, validate inputs at boundary. Every action audit-logged (append-only).

### 3. Frontend
- `src/features/data-rights/` (api wrappers), a "My data & privacy" area on the profile (download export, request erasure, see request status), and an admin queue page. Routes in `App.tsx` (ProtectedRoute / role-gated) + profile tile.

### 4. Tests
- Backend: >= 4 tests (export returns owner-scoped data, erasure request created, admin fulfil anonymises profile, audit-log written, append-only audit/consent records preserved after erasure).
- Frontend: >= 3 tests (renders request state, request export triggers API, admin fulfil action triggers API).

### 5. Execution steps
1. Read CLAUDE.md, then `Features/Consent/` and `Features/Admin/` as templates.
2. Domain/Application -> Infrastructure (impl + DI + migration) -> API -> Frontend -> Tests.
3. `/usr/local/dotnet/dotnet build FundiLink.sln` and `dotnet test` — all green.
4. Frontend `npm run build` and `npm test -- --run` — clean.
5. Update `ROADMAP.md` Phase 10 checklist, `docs/15-api-specification.md`, and `docs/07-privacy-compliance.md` (document export/erasure flows and the retention rule that audit/consent logs are preserved).
6. Write the Phase 11 prompt into `prompts/NEXT_PROMPT.md`.
7. Commit: `Add Phase 10 data subject rights export and erasure (POPIA)`.
8. Push: `git push origin HEAD:claude/happy-dirac-n7qgtg`.

**Security/privacy notes for commit message:**
- Data export is owner-scoped; erasure fulfilment is an audited admin action that anonymises/soft-deletes personal data while preserving append-only audit and consent records (POPIA proof-of-processing retention).
- All export and erasure actions are append-only audit-logged.
- No third-party storage/email/delivery integration — deterministic in-process implementation behind interfaces; any future key via env only.
- No secrets committed.
