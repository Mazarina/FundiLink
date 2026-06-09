# NEXT_PROMPT.md — Phase 2: Student Profile, Academic Record, and APS Calculator

## When to Run This Prompt
Run this after Phase 1 scaffold is committed and the solution builds cleanly with tests passing.
This prompt implements the core learner registration, profile, and APS calculation flow.

---

## Prompt 002 — Student Profile, Academic Record, and APS Calculator

---

Read `CLAUDE.md`, `PRODUCT_REQUIREMENTS.md`, and `ARCHITECTURE.md` before starting.
Follow all rules in `CLAUDE.md` — especially: no secrets committed, POPIA consent required, RBAC enforced.

You are working on FundiLink by ZulTek. Phase 1 scaffold is complete. You are now building Phase 2: the core learner registration, profile creation, academic record entry, and APS calculation.

**Stack:** ASP.NET Core 8, Clean Architecture, PostgreSQL + EF Core, React + Vite + TypeScript, Tailwind CSS, JWT authentication.

---

## Backend: What to Build

### 1. Domain Entities

In `FundiLink.Domain`:

Create the following entities (all extending `BaseEntity`):

**Learner**
- Fields: UserId (string), FirstName, Surname, DateOfBirth, IdNumber (nullable), PassportNumber (nullable), Gender (nullable), HomeLanguage (nullable), Nationality, MobileNumber, Province, Municipality, Suburb, SchoolName, SchoolProvince, GradeLevel (enum: Grade11/Grade12/PostMatric), GuardianName (nullable), GuardianPhone (nullable), GuardianEmail (nullable), ConsentAccepted (bool), ConsentTimestamp, ConsentVersion, GuardianConsentAccepted (bool, nullable), GuardianConsentTimestamp (nullable), ProfileCompleteness (int 0–100)

**AcademicProfile**
- Fields: LearnerId, Year (int), ResultType (enum: Grade11Prelim/Grade12Final/Grade12Prelim), ApsScore (int), ApsCalculatedAt (nullable)
- Navigation: ICollection<NscSubjectResult> Subjects

**NscSubjectResult**
- Fields: AcademicProfileId, SubjectName, SubjectCode (nullable), Percentage (int 0–100), ApsPoints (int), IsHomeLanguage (bool), IsLifeOrientation (bool)

### 2. APS Calculator

In `FundiLink.Application/Features/AcademicProfile`:

Create an `ApsCalculatorService` with:
- Method: `CalculateAps(IEnumerable<NscSubjectResult> subjects) -> int`
- Standard NSC scale: 80–100=7, 70–79=6, 60–69=5, 50–59=4, 40–49=3, 30–39=2, 0–29=1
- Life Orientation excluded from APS total
- Returns total APS from best 6 subjects (excluding LO)

Write comprehensive unit tests for the APS calculator in `FundiLink.Application.Tests` — test edge cases, LO exclusion, boundary values.

### 3. Application Use Cases (CQRS via MediatR)

Create the following commands and queries:

**Auth:**
- `RegisterLearnerCommand` — create Identity user + Learner record, send verification email (stub the email for now), enforce POPIA consent
- `LoginCommand` — return access token + refresh token
- `RefreshTokenCommand`

**Learner Profile:**
- `GetMyProfileQuery`
- `UpdatePersonalInfoCommand`

**Academic Profile:**
- `GetAcademicProfileQuery` — returns profile + subjects + APS
- `SaveAcademicProfileCommand` — upsert subjects, recalculate APS, persist

### 4. API Controllers

Create controllers in `FundiLink.Api/Controllers`:

- `AuthController` — POST /api/v1/auth/register, /login, /refresh
- `LearnersController` — GET/PUT /api/v1/learners/me
- `AcademicProfileController` — GET/PUT /api/v1/learners/me/academic-profile, GET /api/v1/learners/me/aps

**Security rules:**
- `[Authorize]` on all endpoints except register and login
- Learners can only access their own profile — validate ownership in handlers
- Register requires: consentAccepted = true in the request body

### 5. EF Core

Add the new entities to `FundiLinkDbContext`.
Generate a new migration: `AddLearnerAndAcademicProfile`.
Do NOT run the migration against a real database — just generate it.

### 6. POPIA

Registration must:
- Require `consentAccepted = true` in the request (reject with 400 if false)
- Record `ConsentTimestamp = DateTime.UtcNow`
- Record `ConsentVersion = "1.0"` (constant for now)
- If DOB indicates under-18, set a flag and require guardian info

---

## Frontend: What to Build

### 1. Auth Feature (`/src/features/auth`)

Create:
- `RegisterPage.tsx` — form: firstName, surname, email, password, dateOfBirth, consentAccepted checkbox
- `LoginPage.tsx` — form: email, password
- `authApi.ts` — typed API calls for register, login, refresh
- `AuthContext.tsx` or Zustand auth store — store access token, user info, isAuthenticated
- Protected route component — redirect to /login if not authenticated

### 2. Profile Feature (`/src/features/profile`)

Create:
- `ProfilePage.tsx` — display learner info, completeness indicator
- `EditProfilePage.tsx` — form to edit personal info

### 3. APS Feature (`/src/features/aps`)

Create:
- `AcademicProfilePage.tsx` — enter NSC subjects and marks
- `ApsScoreDisplay.tsx` — show APS score prominently with a breakdown table
- `SubjectSelector.tsx` — dropdown of approved NSC subjects (hardcode the list)
- `academicApi.ts` — typed API calls

### 4. Routing

Update `App.tsx` with routes:
- `/` → `HomePage`
- `/register` → `RegisterPage`
- `/login` → `LoginPage`
- `/profile` → `ProfilePage` (protected)
- `/profile/edit` → `EditProfilePage` (protected)
- `/academic` → `AcademicProfilePage` (protected)
- `*` → `NotFoundPage`

### 5. POPIA Consent UI

On the register page:
- Display a clear, plain-language consent notice above the checkbox
- Checkbox label: "I agree to FundiLink's Privacy Policy and consent to my information being processed as described above."
- The checkbox must be checked to submit
- Include a note: "FundiLink is not an official admissions portal. It helps you prepare and track your applications."

---

## Testing Requirements

**Backend:**
- Unit tests for `ApsCalculatorService` — minimum 10 test cases including LO exclusion, boundary values, all-low scores, all-high scores
- Unit tests for `RegisterLearnerCommand` handler (mock dependencies)
- Integration tests for auth endpoints (register → login → get profile flow)

**Frontend:**
- Unit tests for `ApsScoreDisplay` component — verify it renders the correct APS score
- Unit tests for APS calculation utility if extracted to the frontend

---

## Security Requirements

- No secrets committed
- Passwords never logged or returned in responses
- ID numbers never returned in API responses in plaintext — mask if needed
- JWT tokens validated on all protected endpoints
- Ownership validation: a learner can only GET/PUT their own profile

---

## What NOT to Do

- Do not implement programme matching (Phase 3)
- Do not implement document upload (Phase 3)
- Do not implement admin portal (Phase 4)
- Do not send real emails — stub the email service
- Do not run database migrations against a production database
- Do not commit secrets

---

## Output Requirements

1. Confirm `dotnet build` passes
2. Confirm `dotnet test` passes (including APS unit tests)
3. Confirm `npm run build` passes in `src/fundilink-web`
4. List all files created or modified
5. Confirm no secrets committed
6. Commit with message: `Add Phase 2 learner profile, academic record, and APS calculator`
7. Push to `claude/happy-dirac-n7qgtg`
8. Update `ROADMAP.md` Phase 2 checklist
9. Update this file (`NEXT_PROMPT.md`) with the Phase 3 prompt

---

## Definition of Done for Phase 2

- [ ] Learner can register with POPIA consent
- [ ] Learner can log in and receive JWT token
- [ ] Learner can create and edit their profile
- [ ] Learner can enter NSC results
- [ ] APS score is calculated correctly (validated by unit tests)
- [ ] Academic profile API returns APS score with breakdown
- [ ] Frontend shows registration, login, profile, and APS pages
- [ ] All RBAC rules enforced (own data only)
- [ ] Minimum 10 APS unit tests pass
- [ ] Integration tests for auth flow pass
- [ ] Zero secrets committed
- [ ] Build and tests green
