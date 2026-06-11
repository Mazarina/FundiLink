# Phase 13 — Learner Home Dashboard & Activity Summary (MVP)

You are building Phase 13 of FundiLink by ZulTek on branch `claude/happy-dirac-n7qgtg`.

FundiLink is a South African student opportunity platform (Clean Architecture: ASP.NET Core 8 backend, React+Vite+TypeScript frontend). Phases 1-12 are complete and green (all .NET + frontend tests pass, both build clean).

**FIRST**: Read `/home/user/FundiLink/CLAUDE.md` fully. Key rules:
- NEVER commit secrets, API keys, or credentials. Any third-party key MUST come from environment variables / user-secrets only — placeholder `SET_VIA_ENV` in `appsettings.json`.
- dotnet is at `/usr/local/dotnet/dotnet`.
- Push to `claude/happy-dirac-n7qgtg` only (this worktree may be on its own local branch — push with `git push origin HEAD:claude/happy-dirac-n7qgtg`).
- All existing tests must keep passing (currently 136 .NET, 50 frontend).
- Follow existing CQRS/MediatR patterns (`Features/Reporting/`, `Features/Notifications/`, `Features/DataRights/`, `Features/Consent/`), the typed-DTO pattern, and the append-only POPIA-minimal audit-log pattern (`AuditLogEntry`).

**CRITICAL POSITIONING / SAFETY (POPIA):**
- The dashboard is owner-scoped guidance only — FundiLink is NOT an official admissions/funding portal. Never imply official institution/NSFAS/bursary submission.
- Owner-scoped only: a learner sees only their own data, resolved from the authenticated user id. No new way to read another learner's data; no new PII surface beyond what the learner already owns.
- Read-only aggregation of the learner's own existing data — introduce no new per-learner audit surface beyond existing access patterns.
- No third-party integrations. Any future provider goes behind an interface, key via env only.

## Scope — MVP only
1. **Learner home summary** — a single owner-scoped, read-only query that composes the learner's at-a-glance picture from existing data: profile completeness, counts of programme/bursary applications by status, upcoming deadlines (next N days, reusing the Phase 12 `IDeadlineQueryRepository` pattern but owner-scoped), pending document checklist items, and recent notifications (reuse the Phase 12 history). Aggregate/derived figures only; reuse existing repositories.
2. **Home dashboard page (frontend)** — a learner landing dashboard that renders the summary with clear links into the existing features (applications, bursaries, documents, deadlines, notification history). Mobile-first.

### 1. Domain / Application
- Add `Features/Home/` (or `Features/Dashboard/`) CQRS: a single `GetLearnerHomeSummaryQuery` returning a typed `LearnerHomeSummaryDto`. Reuse existing repositories (`IApplicationRepository`, `IBursaryApplicationRepository`, `IChecklistRepository`, `INotificationLogRepository`, deadline query, profile completeness). Owner-scoped: resolve learner from user id; throw `KeyNotFoundException` if no profile.
- No new entities or migrations expected — this is a read/composition phase. If a new owner-scoped deadline read is needed, extend the existing deadline query in an owner-scoped, minimal way.

### 2. API
- Add to a `HomeController` (or extend an existing learner controller) under `api/v1/...`: `GET home/summary` (owner-scoped, `[Authorize]`). Typed DTO response; no PII beyond the learner's own existing fields.

### 3. Frontend
- `src/pages/HomeDashboardPage.tsx` + `src/features/home/homeApi.ts`. Render summary cards and links. Add a route and make it the post-login landing (or a prominent profile tile). Mobile-first, guidance-only copy.

### 4. Tests
- Backend (>= 4): summary composes expected counts from seeded data, empty/new-learner returns zeros, upcoming deadlines respect the window, owner-scoping (no cross-learner leakage), missing profile throws.
- Frontend (>= 3): dashboard renders summary cards, empty state renders, links navigate to the right routes.

### 5. Execution steps
1. Read CLAUDE.md, then `Features/Reporting/` and `Features/Notifications/` as recent templates, plus the existing learner-scoped query handlers and repositories you will reuse.
2. Application -> API -> Frontend -> Tests.
3. `/usr/local/dotnet/dotnet build FundiLink.sln` and `dotnet test` — all green.
4. Frontend `npm run build` and `npm test -- --run` — clean.
5. Update `ROADMAP.md` Phase 13 checklist, `docs/15-api-specification.md`, and `docs/07-privacy-compliance.md` (note: owner-scoped, read-only composition of the learner's own data, no new PII surface).
6. Write the Phase 14 prompt into `prompts/NEXT_PROMPT.md`.
7. Commit: `Add Phase 13 learner home dashboard and activity summary`.
8. Push: `git push origin HEAD:claude/happy-dirac-n7qgtg`.

**Security/privacy notes for commit message:**
- Owner-scoped, read-only composition of the learner's own existing data; guidance only; no official-portal claims.
- No new PII surface; no new third-party integration.
- No secrets committed.

IMPORTANT NOTE ON PUSHING: this worktree may be on its own local branch (not `claude/happy-dirac-n7qgtg` directly). When pushing, use: `git push origin HEAD:claude/happy-dirac-n7qgtg`
