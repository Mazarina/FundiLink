# Phase 14 — Saved Opportunities & Personalised Next-Steps (MVP)

You are building Phase 14 of FundiLink by ZulTek on branch `claude/happy-dirac-n7qgtg`.

FundiLink is a South African student opportunity platform (Clean Architecture: ASP.NET Core 8 backend, React+Vite+TypeScript frontend). Phases 1-13 are complete and green (all .NET + frontend tests pass, both build clean).

**FIRST**: Read `/home/user/FundiLink/CLAUDE.md` fully. Key rules:
- NEVER commit secrets, API keys, or credentials. Any third-party key MUST come from environment variables / user-secrets only — placeholder `SET_VIA_ENV` in `appsettings.json`.
- dotnet is at `/usr/local/dotnet/dotnet`.
- Push to `claude/happy-dirac-n7qgtg` only (this worktree may be on its own local branch — push with `git push origin HEAD:claude/happy-dirac-n7qgtg`).
- All existing tests must keep passing (currently 142 .NET, 53 frontend).
- Follow existing CQRS/MediatR patterns (`Features/Home/`, `Features/Reporting/`, `Features/Notifications/`, `Features/DataRights/`, `Features/Consent/`), the typed-DTO pattern, and the append-only POPIA-minimal audit-log pattern (`AuditLogEntry`).

**CRITICAL POSITIONING / SAFETY (POPIA):**
- Owner-scoped guidance only — FundiLink is NOT an official admissions/funding portal. Never imply official institution/NSFAS/bursary submission.
- Owner-scoped only: a learner sees and mutates only their own data, resolved from the authenticated user id. No new cross-learner read/write surface.
- Data minimisation: store only the reference (programme/bursary id) and minimal metadata needed; no new sensitive PII.
- No third-party integrations. Any future provider goes behind an interface, key via env only.

## Scope — MVP only
1. **Saved opportunities (shortlist)** — let a learner bookmark programmes and bursaries they are interested in (without yet creating a tracked application), so the Phase 13 dashboard and matches pages can surface a personal shortlist. Owner-scoped CRUD: save, list, remove. A new owner-scoped entity + migration is expected here (e.g. `SavedOpportunity` with `LearnerId`, `OpportunityKind`, `OpportunityId`, `CreatedAt`, soft-delete to match existing entities).
2. **Personalised next-steps** — a small, deterministic, read-only rules pass over the learner's own existing data (incomplete profile, saved-but-not-applied opportunities, pending required documents, imminent deadlines) that returns an ordered list of suggested next actions with a stable type + human-readable label + deep link. No ML, no third party — pure deterministic rules.
3. **Frontend** — a "Saved" page (list + remove), a save/unsave control on programme and bursary detail/match cards, and a "Next steps" panel on the Phase 13 dashboard linking into the relevant feature. Mobile-first, guidance-only copy.

### 1. Domain / Application
- Add `SavedOpportunity` domain entity (owner-scoped, soft-delete, factory `Create`, no setters leaking). Add EF configuration + a migration. Add `ISavedOpportunityRepository` (owner-scoped get/add/remove/exists) and its implementation; register in DI.
- Add `Features/SavedOpportunities/` CQRS: `SaveOpportunityCommand`, `RemoveSavedOpportunityCommand`, `GetMySavedOpportunitiesQuery` (typed DTOs; owner-scoped; resolve learner from user id; `KeyNotFoundException` if no profile; reject duplicates idempotently).
- Add `Features/NextSteps/` CQRS: `GetMyNextStepsQuery` returning a typed `NextStepDto[]` (`type`, `label`, `link`, ordered by priority). Deterministic rules only; reuse existing repositories + the saved-opportunities repo. Consider composing it into the Phase 13 home summary or exposing it separately (your call — document it).

### 2. API
- `SavedOpportunitiesController` under `api/v1/saved-opportunities`: `GET` (list), `POST` (save), `DELETE/{id}` (remove). `[Authorize]`, owner-scoped, inputs validated.
- `GET api/v1/next-steps` (owner-scoped, `[Authorize]`) — or fold next-steps into `GET home/summary` (document whichever you choose).

### 3. Frontend
- `src/features/saved/savedApi.ts`, `src/pages/SavedOpportunitiesPage.tsx`, save/unsave control on programme & bursary cards, "Next steps" panel on `HomeDashboardPage`. Route(s) in `App.tsx`. Mobile-first, guidance-only copy.

### 4. Tests
- Backend (>= 5): save creates owner-scoped record, duplicate save is idempotent, list is owner-scoped (no cross-learner leakage), remove is owner-scoped (cannot remove another learner's save), next-steps rules produce the expected ordered actions for seeded states, missing profile throws.
- Frontend (>= 3): saved list renders + empty state, save/unsave control calls the API and updates UI, next-steps panel renders actions with correct links.

### 5. Execution steps
1. Read CLAUDE.md, then `Features/Home/`, `Features/Bursaries/`, and the existing entity/migration/repository patterns you will mirror.
2. Domain + migration -> Application -> API -> Frontend -> Tests.
3. `/usr/local/dotnet/dotnet build FundiLink.sln` and `dotnet test` — all green.
4. Frontend `npm run build` and `npm test -- --run` — clean.
5. Update `ROADMAP.md` Phase 14 checklist, `docs/15-api-specification.md`, `docs/07-privacy-compliance.md` (owner-scoped CRUD over a new minimal reference entity; deterministic read-only next-steps; no new sensitive PII; no third party), and `ARCHITECTURE.md` if the entity set changes.
6. Write the Phase 15 prompt into `prompts/NEXT_PROMPT.md`.
7. Commit: `Add Phase 14 saved opportunities and personalised next-steps`.
8. Push: `git push origin HEAD:claude/happy-dirac-n7qgtg`.

**Security/privacy notes for commit message:**
- Owner-scoped shortlist CRUD over a new minimal reference entity (programme/bursary id only); deterministic, read-only next-steps composed from the learner's own data; guidance only; no official-portal claims.
- No new sensitive PII surface; no new third-party integration.
- New entity + EF migration added (no destructive changes to existing migrations).
- No secrets committed.
