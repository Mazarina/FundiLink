# Phase 6 — Bursary Hub and Eligibility Matching (MVP)

You are building Phase 6 of FundiLink by ZulTek on branch `claude/happy-dirac-n7qgtg`.

FundiLink is a South African student opportunity platform (Clean Architecture: ASP.NET Core 8 backend, React+Vite+TypeScript frontend). Phases 1–5 are complete and green (all .NET + frontend tests pass, both build clean).

**FIRST**: Read `/home/user/FundiLink/CLAUDE.md` fully. Key rules:
- NEVER commit secrets, API keys, or credentials.
- dotnet is at `/usr/local/dotnet/dotnet`.
- Push to `claude/happy-dirac-n7qgtg` only.
- All existing tests must keep passing.
- Follow existing CQRS/MediatR patterns (see `Features/Applications/`, `Features/ProgrammeMatching/`, `Features/Admin/`) and EF repository/configuration patterns under `Infrastructure/Persistence/`.

**CRITICAL POSITIONING / DISCLAIMERS:**
- FundiLink is NOT an official bursary, NSFAS, or funding admissions platform. Bursary data in this phase is **curated public information for guidance only**. The UI and API responses must make this clear.
- NO real partner integrations, NO payment gateway, NO official funder API calls in this phase — anything touching a third party must be stubbed behind an interface (as Phase 5 did for WhatsApp/SMS).
- Eligibility matching produces **guidance ("you may qualify")**, never a guarantee or an application submission.

---

## Scope — MVP only

A bursary catalogue learners can browse, an eligibility matcher that uses the learner's existing profile + APS, a simple bursary application tracker, and the read-only foundation (entity + admin CRUD) for a future partner portal — no actual partner onboarding flow.

### 1. Domain (`src/FundiLink.Domain/`)
- `Enums/BursaryFundingType.cs`: `{ FullCost, TuitionOnly, PartialTuition, Stipend, Accommodation }`
- `Enums/BursaryStatus.cs` (catalogue visibility): `{ Open, ClosingSoon, Closed }`
- `Enums/BursaryApplicationStatus.cs`: `{ Researching, Preparing, Submitted, Awarded, Rejected }` (separate tracker entity — do NOT overload LearnerApplication)
- `Entities/Bursary.cs` (extends BaseEntity): Name, ProviderName, Description, FundingType, FieldsOfStudy (model like Programme's RequiredSubjects — child collection or delimited string), MinimumAps (int?), MaxHouseholdIncome (decimal?), ProvincesEligible, ApplicationOpenDate/CloseDate (DateTime?), ExternalApplicationUrl (learners apply on the funder's own portal), IsActive. Factory `Create(...)` + `Update(...)`, no public setters (match Programme/Institution style).
- `Entities/BursaryApplication.cs` (extends BaseEntity): LearnerId, BursaryId, BursaryApplicationStatus, Notes, DeadlineDate. Factory + `UpdateStatus(...)` (mirror LearnerApplication).
- Add `BursaryStatusChange` to `Enums/NotificationType.cs`.

### 2. Application (`src/FundiLink.Application/`)
- Interfaces: `IBursaryRepository`, `IBursaryApplicationRepository` (mirror `IProgrammeRepository` / `IApplicationRepository`).
- CQRS in `Features/Bursaries/`:
  - Queries: `GetBursaries` (filter by field of study / province / funding type, active only, disclaimer note in DTO), `GetBursaryById`.
  - Query `GetBursaryMatches` — uses `ILearnerRepository` + the learner's `AcademicProfile.ApsScore`; return bursaries where MinimumAps is null or <= learner APS AND (ProvincesEligible empty or contains learner province). Return a match DTO with `reasons` and `guidanceOnly: true`, consistent with Phase 3 `ProgrammeMatching`.
  - Commands: `CreateBursaryApplication`, `UpdateBursaryApplicationStatus`, `DeleteBursaryApplication` — owner-scoped (enforce the learner owns the record). On status change, call `INotificationService.NotifyAsync` with `NotificationType.BursaryStatusChange`.
  - Query: `GetMyBursaryApplications`.
- Admin CQRS in `Features/Admin/` (mirror CreateProgramme/UpdateProgramme): `CreateBursary`, `UpdateBursary`. RBAC Admin/SuperAdmin. Append-only `AuditLogEntry` for each create/update (reuse `IAuditLogRepository`).

### 3. Infrastructure (`src/FundiLink.Infrastructure/`)
- `BursaryRepository`, `BursaryApplicationRepository` (mirror existing repos).
- EF configs: `BursaryConfiguration` (ToTable("Bursaries"), enums as strings, soft-delete query filter), `BursaryApplicationConfiguration` (ToTable("BursaryApplications"), enums as strings, soft-delete filter).
- Add DbSets to `FundiLinkDbContext`; register repos in `DependencyInjection.cs`.
- Generate migration:
  ```
  /usr/local/dotnet/dotnet ef migrations add AddBursaries \
    --project src/FundiLink.Infrastructure --startup-project src/FundiLink.Api \
    --output-dir Persistence/Migrations
  ```
- Optionally seed 3–5 well-known **public** bursary examples (e.g. NSFAS guidance, Funza Lushaka) via the existing seeding mechanism — clearly marked guidance-only, no fabricated amounts/contact details; keep descriptions generic if unsure.

### 4. API (`src/FundiLink.Api/`)
- `BursariesController` (`api/v1/bursaries`): GET list (filters), GET by id, GET `/matches`. `[Authorize]`.
- Bursary application endpoints `api/v1/bursary-applications` (mirror ApplicationsController): create, list mine, update status, delete.
- Admin bursary CRUD — RBAC `[Authorize(Roles=...)]` Admin/SuperAdmin, matching programme/institution admin protection.
- Add request DTOs to `Models/Requests.cs`.
- Every bursary-facing DTO carries a guidance-only disclaimer field/string.

### 5. Frontend (`src/fundilink-web/`)
- Types in `src/types/index.ts`: `Bursary`, `BursaryMatch`, `BursaryApplication`, enums as string unions.
- API client `src/features/bursaries/bursariesApi.ts`.
- Pages: `BursariesPage` (browse + filter), `BursaryDetailPage` (details + clear "This is guidance only. Apply on the funder's official portal." note + external link), `BursaryMatchesPage` (uses learner APS/profile), `BursaryApplicationsPage` (tracker, mirror ApplicationsPage).
- Routing in `App.tsx` (ProtectedRoute). Add a "Bursary Hub" tile to `ProfilePage`.
- Follow existing Tailwind/brand styling and the messaging/error patterns from `ProgrammesPage`/`ApplicationsPage`.

### 6. Tests
**Backend** in `tests/FundiLink.Application.Tests/Features/Bursaries/` (and `Features/Admin/`):
- `GetBursaryMatchesHandlerTests` — APS at/above minimum matches; below does not; null minimum always matches; province filter respected (>= 4 tests).
- Bursary application handler tests — create, update status (verify `INotificationService` called), owner-scope enforcement (unauthorised learner rejected), delete (>= 4 tests).
- Admin `CreateBursary`/`UpdateBursary` — writes audit log (>= 2 tests).
- Domain tests for `Bursary.Create`/`Update` and `BursaryApplication.UpdateStatus` (>= 2 tests).
**Frontend** — at least 3 tests across new pages (mock the API): renders matches, renders guidance-only disclaimer, tracker status update.

### 7. Execution steps
1. Read `CLAUDE.md`.
2. Explore Phase 3 (`Features/Applications`, `Features/ProgrammeMatching`) and Phase 4 admin patterns as templates.
3. Domain → Application → Infrastructure (+ migration) → API → Frontend → Tests.
4. `/usr/local/dotnet/dotnet build FundiLink.sln` and `dotnet test` — all existing + new tests pass.
5. Frontend `npm run build` and `npm test -- --run` — clean.
6. Update `ROADMAP.md` Phase 6 checklist (mark delivered; leave payment gateway / official integration framework / skills module deferred).
7. Write the Phase 7 prompt into `prompts/NEXT_PROMPT.md` (per ROADMAP — scoped, MVP-only, no real integrations).
8. Commit: `Add Phase 6 bursary hub and eligibility matching`.
9. Push: `git push -u origin claude/happy-dirac-n7qgtg`.

**Security/privacy notes for commit message:**
- No real funder/partner API integrations or payment gateway — guidance-only, stubbed where third parties would be involved.
- Bursary data is curated public information presented as guidance, not official admissions.
- BursaryApplication is owner-scoped; admin bursary writes are audit-logged.
- No secrets committed.

**dotnet is at `/usr/local/dotnet/dotnet`**
**Never commit secrets.**
