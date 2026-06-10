# Phase 8 — Accommodation & Early-Career Opportunities (MVP)

You are building Phase 8 of FundiLink by ZulTek on branch `claude/happy-dirac-n7qgtg`.

FundiLink is a South African student opportunity platform (Clean Architecture: ASP.NET Core 8 backend, React+Vite+TypeScript frontend). Phases 1–7 are complete and green (all .NET + frontend tests pass, both build clean).

**FIRST**: Read `/home/user/FundiLink/CLAUDE.md` fully. Key rules:
- NEVER commit secrets, API keys, or credentials. Any third-party key MUST come from environment variables / user-secrets only — placeholder `SET_VIA_ENV` in `appsettings.json`.
- dotnet is at `/usr/local/dotnet/dotnet`.
- Push to `claude/happy-dirac-n7qgtg` only.
- All existing tests must keep passing.
- Follow existing CQRS/MediatR patterns (`Features/Bursaries/`, `Features/ProgrammeMatching/`, `Features/Assistant/`, `Features/Admin/`), the stub-behind-interface pattern (`StubWhatsAppService`, `RuleBasedAiAssistantService` behind interfaces), the Phase 6/7 disclaimer pattern (`BursaryDisclaimer`, `AssistantDisclaimer`), and the append-only POPIA-minimal log pattern (`AssistantInteractionLog`).

**CRITICAL POSITIONING / SAFETY:**
- FundiLink is NOT an official accommodation provider, landlord, employer, or recruitment agency. All listings are curated public/partner information for guidance only — never guarantee availability, price, safety, or placement.
- No real external listing/job-board provider integration in this phase by default — sit behind an interface with a deterministic, seeded stub (curated public data). A real provider may be wired later behind the same interface, key via env only.
- Every listing-facing response carries a guidance-only disclaimer and a "verify with the provider / talk to a support agent" note.

## Scope — MVP only

Two learner-facing modules grounded in curated FundiLink data:
1. **Student accommodation listings** — browse/filter near an institution or province; "may suit you" guidance based on the learner's province/institution; track interest. No payments, no bookings.
2. **Early-career opportunities** — learnerships, internships, skills programmes; basic eligibility hints from the learner's grade level / field; track interest. No application submission.

### 1. Domain / Application
- New entities (e.g. `AccommodationListing`, `CareerOpportunity`) with append-only/owner-scoped interest tracking, EF configs, DbSets, migration.
- `Features/Accommodation/` and `Features/Career/` CQRS queries (browse/filter, by-id, matches) + interest-tracking commands. Typed DTOs only — no `dynamic`/`object`. Reuse the disclaimer + guidance-only pattern.
- Curated/seeded data behind a repository; no fabricated provider/employer facts.

### 2. API
- `AccommodationController` and `CareerController` under `api/v1/...`, `[Authorize]`, owner-scoped for any learner-specific data. Validate inputs at the boundary.

### 3. Frontend
- `src/features/accommodation/` and `src/features/career/` (api wrappers, disclaimer banners), pages with filter chips/cards, guidance-only disclaimers, support links. Routes in `App.tsx` (ProtectedRoute) + tiles on `ProfilePage`.

### 4. Tests
- Backend: ≥ 4 tests per module (filtering, matches grounded in profile, owner-scope enforced, interest tracking, no-profile path).
- Frontend: ≥ 3 tests per module (renders disclaimer, renders listings from mocked API, filter/interest action triggers API call).

### 5. Execution steps
1. Read CLAUDE.md, then Phase 6/7 features as templates.
2. Domain/Application → Infrastructure (stub + DI + migration) → API → Frontend → Tests.
3. `/usr/local/dotnet/dotnet build FundiLink.sln` and `dotnet test` — all green.
4. Frontend `npm run build` and `npm test -- --run` — clean.
5. Update `ROADMAP.md` Phase 8 checklist and `docs/15-api-specification.md`.
6. Write the Phase 9 prompt into `prompts/NEXT_PROMPT.md`.
7. Commit: `Add Phase 8 accommodation and early-career opportunities (stub)`.
8. Push: `git push -u origin claude/happy-dirac-n7qgtg`.

**Security/privacy notes for commit message:**
- No real external listing/job provider integration — deterministic seeded stub behind interfaces; any future key via env only.
- Listings are curated guidance only; no fabricated provider/employer facts; no bookings or payments.
- Learner-specific data is owner-scoped; sensitive access minimised per POPIA.
- No secrets committed.
