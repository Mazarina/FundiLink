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

### GET /api/v1/notifications
Get the learner's notifications.
- Auth: Student

### PUT /api/v1/notifications/{id}/read
Mark notification as read.
- Auth: Student

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

## Health

### GET /health
Health check — no auth required.
- Response: `{ status: "healthy", timestamp: "..." }`

---

## Next Actions
- Implement endpoints in Phase 1–4
- Keep this document updated as contracts change
- Generate Swagger from actual code; use this as design reference
