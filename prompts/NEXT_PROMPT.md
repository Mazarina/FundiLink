# Phase 12 — Notifications & Deadline Reminders Activation (MVP)

You are building Phase 12 of FundiLink by ZulTek on branch `claude/happy-dirac-n7qgtg`.

FundiLink is a South African student opportunity platform (Clean Architecture: ASP.NET Core 8 backend, React+Vite+TypeScript frontend). Phases 1-11 are complete and green (all .NET + frontend tests pass, both build clean).

**FIRST**: Read `/home/user/FundiLink/CLAUDE.md` fully. Key rules:
- NEVER commit secrets, API keys, or credentials. Any third-party key MUST come from environment variables / user-secrets only — placeholder `SET_VIA_ENV` in `appsettings.json`.
- dotnet is at `/usr/local/dotnet/dotnet`.
- Push to `claude/happy-dirac-n7qgtg` only (this worktree may be on its own local branch — push with `git push origin HEAD:claude/happy-dirac-n7qgtg`).
- All existing tests must keep passing.
- Follow existing CQRS/MediatR patterns (`Features/Admin/`, `Features/DataRights/`, `Features/Consent/`, `Features/Reporting/`), the typed-DTO pattern, and the append-only POPIA-minimal audit-log pattern (`AuditLogEntry`).

**CRITICAL POSITIONING / SAFETY (POPIA):**
- Notifications are guidance/reminders only — FundiLink is NOT an official admissions/funding portal. Never imply official institution/NSFAS/bursary submission.
- Respect existing notification preferences and consent. Do not contact a learner on a channel they have not opted into; honour guardian-consent rules for minors.
- No real third-party delivery provider (email/SMS/WhatsApp) is integrated in this phase — keep using the existing stub services behind their interfaces (`IEmailService`, `ISmsService`, `IWhatsAppService`, `INotificationService`). A real provider may be wired later behind the same interface (key via env only).
- Sensitive sends and admin-triggered runs are append-only audit-logged.

## Scope — MVP only
1. **Deadline reminder generation** — a deterministic, in-process service that, given the current date, finds upcoming application/bursary-application deadlines (within a configurable window) for active learners and produces reminder notifications via the existing `INotificationService`, respecting `NotificationPreference` and consent. Idempotent: do not double-send for the same deadline/day.
2. **Notification history (read)** — learner-scoped query over `NotificationLog` (typed DTO) so learners can see what reminders were generated; admin read for support.
3. **Manual reminder run (admin)** — an RBAC-gated, audit-logged endpoint to trigger a reminder pass (for ops/testing) — still using stubs only.

### 1. Domain / Application
- Reuse existing `NotificationLog`, `NotificationPreference`, `INotificationService`. Add `Features/Notifications/` CQRS: generate-reminders command, get-my-notifications query, admin trigger command. Typed DTOs only.
- Reminder selection/idempotency logic behind an interface (deterministic, in-process). No external scheduler in this phase (a hosted background job / cron may be wired later).

### 2. API
- Extend `NotificationsController` (or add endpoints): learner notification history (owner-scoped); admin trigger reminder run (SupportAgent/Admin/SuperAdmin), `[Authorize]`, validate inputs, audit-log the run.

### 3. Frontend
- `src/features/notifications/` additions: notification history page/list; an admin "run reminders" action on an existing admin page. Role-gated routes + profile tile if needed.

### 4. Tests
- Backend (>= 4): reminders generated for due deadlines, none generated outside the window, preferences/consent respected (suppressed channel), idempotency (no double-send), admin trigger is audit-logged + RBAC rejects non-staff.
- Frontend (>= 3): history renders, empty state renders, admin trigger calls the API.

### 5. Execution steps
1. Read CLAUDE.md, then `Features/Notifications/` (if present), `INotificationService` and stubs, `NotificationLog`/`NotificationPreference`, and `Features/Reporting/` as a recent template.
2. Domain/Application -> Infrastructure (impl + DI) -> API -> Frontend -> Tests.
3. `/usr/local/dotnet/dotnet build FundiLink.sln` and `dotnet test` — all green.
4. Frontend `npm run build` and `npm test -- --run` — clean.
5. Update `ROADMAP.md` Phase 12 checklist, `docs/15-api-specification.md`, and `docs/07-privacy-compliance.md` (note: reminders respect preferences/consent, stubs only, audit-logged, no new PII exposure).
6. Write the Phase 13 prompt into `prompts/NEXT_PROMPT.md`.
7. Commit: `Add Phase 12 notifications and deadline reminders activation`.
8. Push: `git push origin HEAD:claude/happy-dirac-n7qgtg`.

**Security/privacy notes for commit message:**
- Reminders are guidance only; respect notification preferences and consent; no official-portal claims.
- Stub delivery providers only — no real email/SMS/WhatsApp integration; any future key via env only.
- Admin-triggered runs and sensitive sends are append-only audit-logged; RBAC-gated.
- No secrets committed.

IMPORTANT NOTE ON PUSHING: this worktree will be on its own local branch (not `claude/happy-dirac-n7qgtg` directly). When pushing, use: `git push origin HEAD:claude/happy-dirac-n7qgtg`
