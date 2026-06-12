# 15 — API Specification

## Title
FundiLink — REST API Specification

## Purpose
Define the API contracts for FundiLink SmartApply. This document is the source of truth for API design decisions.

---

## General Conventions

- **Base URL:** `/api/v1`
- **Format:** JSON
- **Auth:** `Authorization: Bearer <jwt_token>`
- **Versioning:** URL-based (`/api/v1/...`)
- **Pagination:** `?page=1&pageSize=20`
- **Error format:** RFC 7807 Problem Details

### Standard Response Envelope
```json
{
  "data": { ... },
  "errors": [],
  "meta": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 100
  }
}
```

### Standard Error Response
```json
{
  "type": "https://fundilink.co.za/errors/validation",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "FirstName": ["First name is required."]
  }
}
```

---

## Auth Endpoints

### POST /api/v1/auth/register
Register a new learner.
- Body: `{ email, password, firstName, surname, consentAccepted }`
- Response: `201 Created` with user ID; verification email sent
- No auth required

### POST /api/v1/auth/login
Login and receive JWT tokens.
- Body: `{ email, password }`
- Response: `{ accessToken, refreshToken, expiresIn }`
- No auth required

### POST /api/v1/auth/refresh
Refresh access token using refresh token.
- Body: `{ refreshToken }`
- Response: `{ accessToken, refreshToken, expiresIn }`

### POST /api/v1/auth/logout
Invalidate refresh token.
- Auth required
- Body: `{ refreshToken }`

### POST /api/v1/auth/forgot-password
Request password reset email.
- Body: `{ email }`

### POST /api/v1/auth/reset-password
Reset password with token.
- Body: `{ token, email, newPassword }`

---

## Learner Profile Endpoints

### GET /api/v1/learners/me
Get the authenticated learner's profile.
- Auth: Student role

### PUT /api/v1/learners/me
Update the learner's personal profile.
- Auth: Student role
- Body: profile fields

### GET /api/v1/learners/{id}
Get a specific learner's profile.
- Auth: SupportAgent, Admin, SuperAdmin
- Audit logged

---

## Academic Profile Endpoints

### GET /api/v1/learners/me/academic-profile
Get the learner's academic profile and results.
- Auth: Student

### PUT /api/v1/learners/me/academic-profile
Create or update academic profile and NSC results.
- Auth: Student
- Body: `{ year, resultType, subjects: [{ subjectName, percentage }] }`
- Response includes calculated APS score

### GET /api/v1/learners/me/aps
Get the learner's current APS score with breakdown.
- Auth: Student

---

## Programme Endpoints

### GET /api/v1/programmes
Search and filter programmes.
- Auth: Student (implemented as authenticated; may be made public in future)
- Query: `?keyword=Engineering&type=University&province=Gauteng&page=1&pageSize=20`
- Response: `PagedResult<ProgrammeDto>` — `{ items, total, page, pageSize }`
- NOTE: Programme data is for guidance only — not official admission requirements.

### GET /api/v1/programmes/matches
Get programmes matched to the authenticated learner's APS and subjects.
- Auth: Student
- Returns `ProgrammeMatchDto[]` with `isEligible`, `apsGap`, and `missingSubjects`
- Returns an empty list if the learner has no academic profile.

### GET /api/v1/programmes/{id}
Get detailed programme information (includes required subjects).
- Auth: Student

---

## Application Endpoints

### GET /api/v1/applications
Get the learner's application tracker.
- Auth: Student

### GET /api/v1/applications/{id}
Get a single tracked application.
- Auth: Student (own applications only)

### POST /api/v1/applications
Add a new application to the tracker.
- Auth: Student
- Body: `{ programmeId, status, notes?, deadlineDate? }`
- Returns: `201 Created` with `{ id }`

### PUT /api/v1/applications/{id}/status
Update an application's status and notes.
- Auth: Student (own applications only)
- Body: `{ newStatus, notes? }`

### DELETE /api/v1/applications/{id}
Remove an application from the tracker (soft delete).
- Auth: Student (own applications only)

---

## Bursary Endpoints
All bursary responses carry a guidance-only disclaimer. Bursary data is curated public
information for guidance only — FundiLink is not an official bursary/NSFAS/funding platform.

### GET /api/v1/bursaries
List active bursaries. Optional filters: `fieldOfStudy`, `province`, `fundingType`.
- Auth: Authenticated

### GET /api/v1/bursaries/{id}
Bursary detail (includes external funder portal URL where available).
- Auth: Authenticated

### GET /api/v1/bursaries/matches
Bursaries the learner may qualify for, based on their APS and province (guidance only).
- Auth: Authenticated (own profile)

### POST /api/v1/bursary-applications
Start tracking a bursary application.
- Auth: Student (creates an owner-scoped record)

### GET /api/v1/bursary-applications
List the caller's tracked bursary applications.
- Auth: Student (own records only)

### PUT /api/v1/bursary-applications/{id}/status
Update tracked bursary application status. Triggers a BursaryStatusChange notification.
- Auth: Student (own records only)

### DELETE /api/v1/bursary-applications/{id}
Remove a tracked bursary application (soft delete).
- Auth: Student (own records only)

### POST /api/v1/admin/bursaries
Create a bursary (curated public guidance data). Append-only audit-logged.
- Auth: Admin, SuperAdmin

### PUT /api/v1/admin/bursaries/{id}
Update a bursary. Append-only audit-logged.
- Auth: Admin, SuperAdmin

---

## Document Endpoints

### GET /api/v1/documents
Get the learner's document list.
- Auth: Student

### POST /api/v1/documents/upload
Upload a document.
- Auth: Student
- Body: multipart/form-data with file and metadata
- Returns document metadata

### GET /api/v1/documents/{id}/download
Download a document file.
- Auth: Student (own documents), SupportAgent (with audit log)

### DELETE /api/v1/documents/{id}
Soft-delete a document.
- Auth: Student (own documents)

---

## Notification Endpoints

### GET /api/v1/notifications/preferences
Get the learner's notification channel preferences (email/WhatsApp/SMS).
- Auth: Student (owner-scoped)

### PUT /api/v1/notifications/preferences
Update the learner's notification channel preferences.
- Auth: Student (owner-scoped)
- Request body: `{ "emailEnabled": bool, "whatsAppEnabled": bool, "smsEnabled": bool }`

### GET /api/v1/notifications/history
Owner-scoped notification history over the append-only `NotificationLog`.
- Auth: Student (owner-scoped — resolved from the authenticated user id)
- Response: array of `{ id, notificationType, channel, status, sentAt, errorMessage }`
- Read-only. Surfaces no PII beyond the existing log fields; the recipient address is deliberately omitted from the DTO.

### POST /api/v1/notifications/admin/run-deadline-reminders
Admin/ops-triggered deadline-reminder pass (for operations and testing).
- Auth: SupportAgent, Admin, SuperAdmin (RBAC at boundary)
- Request body: `{ "windowDays": number }` — clamped server-side to 1..90 (default 14 if out of range)
- Response: `{ learnersWithUpcomingDeadlines, remindersSent, remindersSkippedAlreadySent }` (aggregate counts only)
- Deterministic, in-process generation over upcoming programme/bursary application deadlines for active
  learners. Honours each learner's notification preferences (via `INotificationService`) and, for minor
  learners, requires a current guardian data-processing consent. Idempotent: at most one deadline reminder
  per learner per UTC day. Reminders are guidance only — FundiLink is NOT an official admissions/funding
  portal. Stub delivery providers only (no real email/SMS/WhatsApp; any future key via environment only).
  Each run is append-only audit-logged (`TriggerDeadlineReminders`). No external scheduler is wired in this phase.

---

## Assistant Endpoints

### POST /api/v1/assistant/ask
Ask the AI guidance assistant a constrained, profile-aware question.
- Auth: Student (owner-scoped — only the caller's own FundiLink data is used)
- Request body: `{ "intent": "WhatIsMyAps" | "WhatDoIQualifyFor" | "WhichBursariesFitMe" | "WhatDocumentsDoINeed" }`
- Response: `{ intent, answer, sources[], guidanceOnly: true, disclaimer }`
- Invalid intent returns `400 Bad Request`.
- Guidance only: answers are grounded strictly in the learner's own FundiLink data via a deterministic
  rule-based service (`IAiAssistantService`). No external LLM call and no fabricated institution,
  programme, bursary, or NSFAS facts. Each interaction is logged append-only (POPIA-minimal: intent + timestamp).

---

## Accommodation Endpoints

All accommodation data is curated public/example information for guidance only. FundiLink is NOT an
accommodation provider, landlord, or booking agent. No bookings or payments occur. Every response
carries a guidance-only disclaimer.

### GET /api/v1/accommodation
Browse/filter accommodation listings.
- Auth: Student (any authenticated user)
- Query: `province?`, `nearInstitution?`, `accommodationType?` (`ResidenceOnCampus` | `PrivateStudentResidence` | `SharedHouse` | `Room` | `Other`)
- Response: `AccommodationListingDto[]`

### GET /api/v1/accommodation/matches
"May suit you" listings grounded in the learner's province/institution (guidance-only).
- Auth: Student (owner-scoped — uses only the caller's own profile)
- Response: `AccommodationMatchDto[]` (each with `reasons[]`, `guidanceOnly: true`, `disclaimer`)

### GET /api/v1/accommodation/interests
List the caller's tracked accommodation interests.
- Auth: Student (owner-scoped)
- Response: `AccommodationInterestSummaryDto[]`

### GET /api/v1/accommodation/{id}
Get a single accommodation listing.
- Auth: Student
- Returns `404 Not Found` if missing.

### POST /api/v1/accommodation/interests
Track interest in a listing.
- Auth: Student (owner-scoped)
- Request body: `{ "accommodationListingId": "guid", "status": "Saved" | "Contacted" | "Applied" | "NotInterested", "notes?": "..." }`
- Response: `201 Created` with `{ id }`

### PUT /api/v1/accommodation/interests/{id}/status
Update the status of a tracked interest.
- Auth: Student (owner-scoped — only the owner may update)
- Request body: `{ "newStatus": "Saved" | "Contacted" | "Applied" | "NotInterested", "notes?": "..." }`
- Response: `204 No Content`

---

## Career Endpoints

All career opportunity data is curated public/example information for guidance only. FundiLink is NOT
an employer or recruitment agency and does not guarantee placement. Applications happen on the
provider's official channel. Every response carries a guidance-only disclaimer.

### GET /api/v1/career
Browse/filter early-career opportunities.
- Auth: Student (any authenticated user)
- Query: `fieldOfInterest?`, `province?`, `opportunityType?` (`Learnership` | `Internship` | `SkillsProgramme` | `Apprenticeship` | `EntryLevelJob`)
- Response: `CareerOpportunityDto[]`

### GET /api/v1/career/matches
Opportunities matched on the learner's grade level / field (guidance-only).
- Auth: Student (owner-scoped — uses only the caller's own profile)
- Response: `CareerMatchDto[]` (each with `reasons[]`, `guidanceOnly: true`, `disclaimer`)

### GET /api/v1/career/interests
List the caller's tracked career interests.
- Auth: Student (owner-scoped)
- Response: `CareerInterestSummaryDto[]`

### GET /api/v1/career/{id}
Get a single career opportunity.
- Auth: Student
- Returns `404 Not Found` if missing.

### POST /api/v1/career/interests
Track interest in an opportunity.
- Auth: Student (owner-scoped)
- Request body: `{ "careerOpportunityId": "guid", "status": "Saved" | "Contacted" | "Applied" | "NotInterested", "notes?": "..." }`
- Response: `201 Created` with `{ id }`

### PUT /api/v1/career/interests/{id}/status
Update the status of a tracked interest.
- Auth: Student (owner-scoped — only the owner may update)
- Request body: `{ "newStatus": "Saved" | "Contacted" | "Applied" | "NotInterested", "notes?": "..." }`
- Response: `204 No Content`

---

## Admin Endpoints

### GET /api/v1/admin/learners
Search learners.
- Auth: SupportAgent, Admin

### GET /api/v1/admin/learners/{id}
Get a specific learner's profile.
- Auth: SupportAgent, Admin
- Audit logged

### POST /api/v1/admin/learners/{id}/notes
Add a support note to a learner's record.
- Auth: SupportAgent, Admin

### GET /api/v1/admin/institutions
Manage institutions.
- Auth: Admin, SuperAdmin

### POST /api/v1/admin/institutions
Create institution.
- Auth: Admin

### PUT /api/v1/admin/institutions/{id}
Update institution.
- Auth: Admin

### GET /api/v1/admin/audit-logs
View audit logs.
- Auth: SuperAdmin only

---

## School Endpoints

### GET /api/v1/schools/me/learners
Get the school's learner list.
- Auth: SchoolAdmin

### GET /api/v1/schools/me/dashboard
Get aggregate school dashboard metrics.
- Auth: SchoolAdmin

---

## Consent & Guardian Co-Access Endpoints (Phase 9)

All endpoints require `[Authorize]`. Learner consent endpoints are owner-scoped (the learner
is resolved from the authenticated user). Guardian endpoints are gated by a guardian link AND a
current `GuardianCoAccess` consent, and return only the consented, minimised scope. All consent
grants/revocations, guardian links, and guardian access are append-only audit-logged. No real
identity-verification / e-signature provider integration — deterministic stub behind the interface.

### GET /api/v1/consent/state
Effective consent state for the caller's own learner profile (per consent type).
- Auth: authenticated learner
- Response: `{ isMinor, guardianConsentRequired, consents: [{ consentType, isGranted, scope, guardianName, recordedAt }], disclaimer }`

### GET /api/v1/consent/history
Full append-only consent history for the caller, newest first.
- Auth: authenticated learner

### POST /api/v1/consent
Record (grant) a guardian consent. Only allowed for learners under 18.
- Body: `{ consentType, scope, guardianName, guardianContact }`

### POST /api/v1/consent/revoke
Record a revocation of a previously granted consent (right to withdraw). Appended as a new record.
- Body: `{ consentType }`

### POST /api/v1/consent/guardian-links
Link a guardian (existing user) to the caller's own minor learner profile (idempotent).
- Body: `{ guardianUserId, guardianName, guardianContact }`

### GET /api/v1/consent/guardian/learners
List the learners the caller (as a guardian) is linked to, with current consent state.
- Auth: authenticated guardian

### GET /api/v1/consent/guardian/learners/{learnerId}
Minimised, read-only co-access view of a linked learner. 403 if not linked or no current consent.
Response is limited to the consented scope; never the ID number, documents, or learner contact details.
- Auth: authenticated guardian (consent-gated)

---

## Data Subject Rights (POPIA — export & erasure)

Learner endpoints are owner-scoped (data resolved from the authenticated user id). Admin
endpoints are RBAC-gated. Every export generation and every erasure request/approve/reject/
fulfil action is append-only audit-logged. Erasure fulfilment anonymises/soft-deletes the
learner's personal data while preserving append-only audit and consent records (POPIA proof-
of-processing retention). No third-party storage/email/delivery integration in this phase.

### GET /api/v1/data-rights/export
Returns a typed, owner-scoped export of the caller's FundiLink data (profile, academic profile,
applications, bursary applications, document metadata, accommodation/career interests, consent
history) plus a POPIA disclaimer. Generated in-process.
- Auth: authenticated learner (owner-scoped)

### GET /api/v1/data-rights/erasure-requests
Lists the caller's own erasure requests, newest first.
- Auth: authenticated learner (owner-scoped)

### POST /api/v1/data-rights/erasure-requests
Raises an erasure request for the caller's own profile. Body: `{ reason?: string }`. Rejected if
an open (Requested/Approved) request already exists. Returns `201` with `{ id }`.
- Auth: authenticated learner (owner-scoped)

### GET /api/v1/data-rights/admin/erasure-requests/pending
Lists erasure requests awaiting review (status Requested), newest first.
- Auth: SupportAgent, Admin, SuperAdmin

### POST /api/v1/data-rights/admin/erasure-requests/{id}/approve
Approves a pending request (does not delete data). Body: `{ note?: string }`. Returns `204`.
- Auth: SupportAgent, Admin, SuperAdmin

### POST /api/v1/data-rights/admin/erasure-requests/{id}/reject
Rejects a pending request. Body: `{ note?: string }`. Returns `204`.
- Auth: SupportAgent, Admin, SuperAdmin

### POST /api/v1/data-rights/admin/erasure-requests/{id}/fulfil
Fulfils the request — anonymises/soft-deletes the learner's personal data while preserving
append-only audit and consent records — and marks it Fulfilled. Body: `{ note?: string }`.
Returns `204`.
- Auth: Admin, SuperAdmin

---

## Reporting (Phase 11)

Read-only admin reporting. Dashboard and POPIA summary expose aggregate figures only (no raw
learner PII). The audit activity report is a filtered view over the existing append-only audit
log. All endpoints are read-only and RBAC-gated; no new way to read learner sensitive fields is
introduced. No third-party analytics/telemetry provider is involved.

### GET /api/v1/reporting/dashboard
Returns aggregate operational figures: `totalLearners`, `learnersByProvince[]`,
`applicationsByStatus[]`, `bursaryApplicationsByStatus[]`, `documentsByStatus[]`,
`pendingDocumentVerifications`, `pendingErasureRequests`, `consentGrants`, `consentRevocations`.
Grouped figures are `{ category, count }` arrays.
- Auth: SupportAgent, Admin, SuperAdmin

### GET /api/v1/reporting/popia-summary
Returns the open POPIA work-queue counts: `{ pendingDocumentVerifications, pendingErasureRequests }`.
- Auth: SupportAgent, Admin, SuperAdmin

### GET /api/v1/reporting/audit-activity
Filtered, paged view over the append-only audit log. Query: `action?`, `actorRole?`, `from?`
(UTC), `to?` (UTC), `page` (default 1), `pageSize` (default 50, max 200). Returns a
`PagedResult<AuditLogEntry>`. Read-only.
- Auth: SuperAdmin

---

## Home (Learner Dashboard)

### GET /api/v1/home/summary
Owner-scoped, read-only at-a-glance summary for the authenticated learner's home dashboard.
Composed entirely from the learner's own existing data — no new PII surface. Returns
`LearnerHomeSummaryDto`: `firstName`, `profileCompleteness` (0-100),
`programmeApplicationCounts[]` and `bursaryApplicationCounts[]` (each `{ status, count }`),
`programmeApplicationTotal`, `bursaryApplicationTotal`, `pendingDocumentCount` (required,
unlinked checklist items across the learner's applications), `upcomingDeadlines[]`
(`{ kind, opportunityName, deadlineDate }`, next 30 days), and `recentNotifications[]`
(`{ id, notificationType, channel, status, sentAt }`, most recent first). Guidance only —
FundiLink is not an official admissions or funding portal.
- Auth: any authenticated learner (owner-scoped; resolves the learner from the token).
- 404 when the user has no learner profile.

---

## Health

### GET /health
Health check — no auth required.
- Response: `{ status: "healthy", timestamp: "..." }`

---

## Next Actions
- Implement endpoints in Phase 1–4
- Keep this document updated as contracts change
- Generate Swagger from actual code; use this as design reference
