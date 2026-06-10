# Phase 9 — Guardian Consent & Co-Access (POPIA, MVP)

You are building Phase 9 of FundiLink by ZulTek on branch `claude/happy-dirac-n7qgtg`.

FundiLink is a South African student opportunity platform (Clean Architecture: ASP.NET Core 8 backend, React+Vite+TypeScript frontend). Phases 1–8 are complete and green (all .NET + frontend tests pass, both build clean).

**FIRST**: Read `/home/user/FundiLink/CLAUDE.md` fully. Key rules:
- NEVER commit secrets, API keys, or credentials. Any third-party key MUST come from environment variables / user-secrets only — placeholder `SET_VIA_ENV` in `appsettings.json`.
- dotnet is at `/usr/local/dotnet/dotnet`.
- Push to `claude/happy-dirac-n7qgtg` only.
- All existing tests must keep passing.
- Follow existing CQRS/MediatR patterns (`Features/Bursaries/`, `Features/Accommodation/`, `Features/Career/`, `Features/Assistant/`, `Features/Admin/`), the stub-behind-interface pattern (`StubWhatsAppService`, `RuleBasedAiAssistantService`), the disclaimer pattern, and the append-only POPIA-minimal log pattern (`AssistantInteractionLog`, notification logs).

**CRITICAL POSITIONING / SAFETY (POPIA):**
- FundiLink processes minors' data. Where a learner is under 18 (`profile.isMinor`), guardian consent is legally required before sensitive processing/sharing. This phase makes that consent explicit, recorded, and auditable.
- No real identity-verification or e-signature provider integration in this phase — sit behind an interface with a deterministic stub. A real provider may be wired later behind the same interface, key via env only.
- Guardian co-access is strictly scoped and consent-gated — a guardian sees only what consent permits, never more (data minimisation). All consent grants/revocations and guardian access are append-only audit-logged.

## Scope — MVP only

A consent and guardian co-access module grounded in the learner's own FundiLink data:
1. **Guardian consent records** — capture, store, and display guardian consent for a minor learner (consent type, scope, granted/revoked timestamps, recorded guardian identity). Append-only history; revocation supported (right to withdraw).
2. **Guardian co-access (read-only, consent-gated)** — a guardian linked to a minor learner can view a minimised, consent-scoped view of the learner's profile/applications. No edit, no document download beyond consented scope.
3. **Consent prompts at collection points** — surface consent state on the learner profile; block/flag sensitive sharing actions when required consent is absent.

### 1. Domain / Application
- New entities (e.g. `GuardianConsent`, `GuardianLink`) with append-only consent history and owner/guardian scoping, EF configs, DbSets, migration.
- `Features/Consent/` CQRS: record consent, revoke consent, query consent state, list consent history; guardian-scoped read queries. Typed DTOs only — no `dynamic`/`object`.
- Consent-check application service behind an interface; deterministic, no external call.

### 2. API
- `ConsentController` (and guardian co-access endpoints) under `api/v1/...`, `[Authorize]`, owner/guardian-scoped. Validate inputs at the boundary. Every sensitive action audit-logged (append-only).

### 3. Frontend
- `src/features/consent/` (api wrappers, consent banners/badges), consent management UI on `ProfilePage`/profile area, guardian read-only view. Routes in `App.tsx` (ProtectedRoute) + tile/section as appropriate.

### 4. Tests
- Backend: ≥ 4 tests (record consent, revoke consent updates state, guardian access blocked without consent, guardian access scoped/minimised with consent, audit-log written).
- Frontend: ≥ 3 tests (renders consent state, record/revoke action triggers API call, guardian view renders minimised data).

### 5. Execution steps
1. Read CLAUDE.md, then Phase 6/7/8 features as templates.
2. Domain/Application → Infrastructure (stub + DI + migration) → API → Frontend → Tests.
3. `/usr/local/dotnet/dotnet build FundiLink.sln` and `dotnet test` — all green.
4. Frontend `npm run build` and `npm test -- --run` — clean.
5. Update `ROADMAP.md` Phase 9 checklist, `docs/15-api-specification.md`, and `docs/07-privacy-compliance.md` (document consent/data flows).
6. Write the Phase 10 prompt into `prompts/NEXT_PROMPT.md`.
7. Commit: `Add Phase 9 guardian consent and co-access (POPIA, stub)`.
8. Push: `git push origin HEAD:claude/happy-dirac-n7qgtg`.

**Security/privacy notes for commit message:**
- Guardian consent and co-access are consent-gated and data-minimised per POPIA; guardians see only the consented scope.
- All consent grants/revocations and guardian access are append-only audit-logged.
- No real identity-verification / e-signature provider integration — deterministic stub behind interfaces; any future key via env only.
- No secrets committed.
