# NEXT_PROMPT.md — Phase 3: Programme Matching and Application Tracker

## When to Run This Prompt
Run this after Phase 2 is committed and all tests pass.
Phase 2 is complete: learner registration, profile, academic record entry, and APS calculator are all implemented and tested.

---

## Prompt 003 — Programme Matching and Application Tracker

---

Read `CLAUDE.md`, `PRODUCT_REQUIREMENTS.md`, and `ARCHITECTURE.md` before starting.
Follow all rules in `CLAUDE.md` — especially: no secrets committed, RBAC enforced, no claims of official integration.

You are working on FundiLink by ZulTek. Phase 2 is complete. You are now building Phase 3: programme matching (using APS + subject requirements) and the application tracker.

**Stack:** ASP.NET Core 8, Clean Architecture, PostgreSQL + EF Core, React + Vite + TypeScript, Tailwind CSS, JWT authentication.

---

## Backend: What to Build

### 1. Domain Entities

In `FundiLink.Domain`, create:

**Institution**
- Fields: Name, InstitutionType (enum: University, TVET, SkillsCentre), Province, Website (nullable), IsActive (bool)

**Programme**
- Fields: InstitutionId, Name, FacultyOrSchool (nullable), NFQLevel (int nullable), MinimumAps (int), RequiredSubjects (JSON: list of {SubjectName, MinimumPercentage}), ApplicationOpenDate (nullable), ApplicationCloseDate (nullable), IsActive (bool)

**LearnerApplication**
- Fields: LearnerId, ProgrammeId, Status (enum: Interested, InProgress, Submitted, Accepted, Rejected, Waitlisted), Notes (nullable), DeadlineDate (nullable), SubmittedAt (nullable), OutcomeAt (nullable)

### 2. Programme Matching Engine

In `FundiLink.Application/Features/ProgrammeMatching`:

Create `ProgrammeMatchingService`:
- Method: `GetMatchingProgrammes(AcademicProfile profile, IEnumerable<Programme> programmes) -> IEnumerable<ProgrammeMatch>`
- A programme matches if:
  - `profile.ApsScore >= programme.MinimumAps`
  - All required subjects have the required minimum percentage in the learner's subjects
- Return a `ProgrammeMatch` DTO: Programme, InstitutionName, IsEligible, MissingAps (int), MissingSubjects (list)

Write unit tests for the matching engine — test: eligible match, APS too low, missing subject, missing subject percentage.

### 3. Application Use Cases (CQRS via MediatR)

**Programmes:**
- `SearchProgrammesQuery` — filter by institution type, province, name keyword, min APS; returns paged results
- `GetProgrammeByIdQuery`
- `GetMatchingProgrammesQuery` — uses the matching engine with the learner's academic profile

**Applications:**
- `CreateApplicationCommand` — learner starts tracking an application
- `UpdateApplicationStatusCommand`
- `GetMyApplicationsQuery` — returns all applications for the authenticated learner
- `GetApplicationByIdQuery`
- `DeleteApplicationCommand` — soft delete

### 4. API Controllers

- `ProgrammesController` — GET /api/v1/programmes (search), GET /api/v1/programmes/{id}, GET /api/v1/programmes/matches
- `ApplicationsController` — POST /api/v1/applications, GET /api/v1/applications, GET /api/v1/applications/{id}, PUT /api/v1/applications/{id}/status, DELETE /api/v1/applications/{id}

Security:
- `[Authorize]` on all endpoints
- Learners can only access their own applications
- `/matches` uses the learner's own academic profile

### 5. Seed Data

Create a seed file with at least 5 South African universities, 3 TVETs, and 10 programmes with realistic APS requirements.
IMPORTANT: This is sample/educational data only — not official admission requirements. Add a disclaimer in the seed file and API responses.

### 6. EF Core

Add the new entities to `FundiLinkDbContext`.
Generate a new migration: `AddProgrammesAndApplications`.
Do NOT run the migration against a production database.

---

## Frontend: What to Build

### 1. Programme Search (`/programmes`)

- `ProgrammesPage.tsx` — search and filter programmes, display results as cards
- `ProgrammeCard.tsx` — shows institution name, programme name, APS requirement, eligibility indicator (green tick / amber warning based on learner's APS)
- `ProgrammeDetailPage.tsx` — full programme details, required subjects, deadline, "Track Application" button

### 2. Programme Matches (`/matches`)

- `MatchesPage.tsx` — shows programmes the learner qualifies for based on their APS
- Filter: by institution type (University / TVET), by province
- Each match shows: programme name, institution, APS gap (0 if met), missing subjects if any

### 3. Application Tracker (`/applications`)

- `ApplicationsPage.tsx` — list of all tracked applications with current status
- `ApplicationCard.tsx` — shows programme, institution, status badge (colour-coded), deadline countdown if set
- Status badge colours: Interested=gray, InProgress=blue, Submitted=yellow, Accepted=green, Rejected=red, Waitlisted=orange
- `ApplicationDetailPage.tsx` — full details, status update form, notes field

### 4. Routing

Add to `App.tsx`:
- `/programmes` → `ProgrammesPage` (protected)
- `/programmes/:id` → `ProgrammeDetailPage` (protected)
- `/matches` → `MatchesPage` (protected)
- `/applications` → `ApplicationsPage` (protected)
- `/applications/:id` → `ApplicationDetailPage` (protected)

Update `ProfilePage.tsx` navigation tiles to include links to `/programmes`, `/matches`, and `/applications`.

### 5. Disclaimer UI

Add a visible disclaimer on all programme and match pages:
"Programme information is provided for guidance only. APS requirements and deadlines may change. Always verify with the official institution. FundiLink is not an official admissions portal."

---

## Testing Requirements

**Backend:**
- Unit tests for `ProgrammeMatchingService` — minimum 8 test cases
- Unit tests for `CreateApplicationCommand` handler
- Integration test: search programmes endpoint returns results

**Frontend:**
- Unit tests for `ProgrammeCard` component — renders eligibility indicator correctly
- Unit tests for status badge colour mapping

---

## Security Requirements

- No secrets committed
- Learners access only their own applications
- No endpoint returns another learner's application data
- Programme seed data includes disclaimer — never claim official APS requirements

---

## What NOT to Do

- Do not implement document upload (Phase 4)
- Do not implement admin portal (Phase 4)
- Do not send real emails
- Do not run migrations against a production database
- Do not claim official admission requirements — always disclaim
- Do not implement payment or bursary matching (Phase 5)

---

## Output Requirements

1. Confirm `dotnet build` passes
2. Confirm `dotnet test` passes
3. Confirm `npm run build` passes in `src/fundilink-web`
4. List all files created or modified
5. Confirm no secrets committed
6. Commit with message: `Add Phase 3 programme matching and application tracker`
7. Push to `claude/happy-dirac-n7qgtg`
8. Update `ROADMAP.md` Phase 3 checklist
9. Update this file (`NEXT_PROMPT.md`) with the Phase 4 prompt

---

## Definition of Done for Phase 3

- [ ] Institutions and programmes seed data in place
- [ ] Programme search and filter endpoint works
- [ ] Matching engine returns correct eligible programmes for a learner's APS
- [ ] Learner can create and track applications
- [ ] Application status updates work
- [ ] Frontend shows programme search, matches, and application tracker
- [ ] Disclaimer shown on all programme/match pages
- [ ] All RBAC rules enforced (own applications only)
- [ ] Minimum 8 matching engine unit tests pass
- [ ] Build and tests green
- [ ] Zero secrets committed
