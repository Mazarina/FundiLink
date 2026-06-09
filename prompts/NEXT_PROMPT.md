# NEXT_PROMPT.md — Phase 4: Document Vault and Admin Portal

## When to Run This Prompt
Run this after Phase 3 is committed and all tests pass.
Phase 3 is complete: programme matching (APS + subject requirements), programme search, and the application tracker (create, update, view, delete) are all implemented and tested.

---

## Prompt 004 — Document Vault and Admin Portal

---

Read `CLAUDE.md`, `PRODUCT_REQUIREMENTS.md`, and `ARCHITECTURE.md` before starting.
Follow all rules in `CLAUDE.md` — especially: no secrets committed, RBAC enforced, POPIA privacy-by-design, audit logs on all sensitive admin actions, no claims of official integration.

You are working on FundiLink by ZulTek. Phase 3 is complete. You are now building Phase 4: a secure document vault for learners and an admin/support portal for FundiLink staff.

**Stack:** ASP.NET Core 8, Clean Architecture, PostgreSQL + EF Core, React + Vite + TypeScript, Tailwind CSS, JWT authentication. Roles already exist: Student, SchoolAdmin, SupportAgent, Admin, SuperAdmin.

---

## Backend: What to Build

### 1. Domain Entities

**Document** (extends BaseEntity)
- LearnerId (Guid), DocumentType (enum: IdDocument, MatricCertificate, AcademicResults, ProofOfResidence, GuardianConsent, Other), FileName (string), ContentType (string), StorageKey (string), SizeBytes (long), Status (enum: Pending, Verified, Rejected), UploadedAt (DateTime), VerifiedAt (DateTime?), VerifiedByUserId (string?), RejectionReason (string?)

**DocumentChecklistItem** (extends BaseEntity)
- LearnerApplicationId (Guid), DocumentType (enum), IsRequired (bool), LinkedDocumentId (Guid?) — links a checklist requirement to an uploaded document

**AuditLogEntry** (extends BaseEntity)
- ActorUserId (string), ActorRole (string), Action (string), TargetType (string), TargetId (string), MetadataJson (string?), OccurredAt (DateTime)
- IMPORTANT: audit log entries are append-only. Never allow update or delete.

**Enums:** `DocumentType`, `DocumentStatus` in `FundiLink.Domain/Enums/`.

### 2. Document Storage Abstraction

- `IDocumentStorageService` in Application/Common/Interfaces with: `StoreAsync(stream, contentType, key, ct)`, `GetAsync(key, ct)`, `DeleteAsync(key, ct)`.
- Implement `LocalDiskDocumentStorageService` in Infrastructure for MVP. Store under a configurable path (`DocumentStorage:RootPath`, default to a local folder; never a secret).
- Documents must NEVER be publicly accessible. All access goes through an authorized API endpoint that streams the file after verifying ownership/role.
- Validate uploads: allowed content types (pdf, jpg, png), max size (configurable, e.g. 10 MB). Reject anything else.

### 3. Application Use Cases (CQRS via MediatR)

**Documents (learner):**
- `UploadDocumentCommand` — validate type/size, store, create Document record (Status=Pending)
- `GetMyDocumentsQuery`
- `DownloadDocumentQuery` — returns stream + metadata; ownership enforced
- `DeleteDocumentCommand` — soft delete + remove from storage (POPIA right to erasure)

**Checklist:**
- `GetApplicationChecklistQuery` — checklist items per application with linked document status
- `LinkDocumentToChecklistCommand`

**Admin / Support (RBAC enforced, audit-logged):**
- `SearchLearnersQuery` (SupportAgent, Admin) — paged; writes an audit log entry
- `GetLearnerOverviewQuery` (SupportAgent, Admin) — read learner profile + application summary; audit-logged
- `VerifyDocumentCommand` / `RejectDocumentCommand` (SupportAgent, Admin) — audit-logged
- `CreateInstitutionCommand`, `UpdateInstitutionCommand`, `CreateProgrammeCommand`, `UpdateProgrammeCommand` (Admin) — audit-logged
- `GetAuditLogQuery` (SuperAdmin only)

### 4. API Controllers

- `DocumentsController` — POST /api/v1/documents (multipart), GET /api/v1/documents, GET /api/v1/documents/{id}/download, DELETE /api/v1/documents/{id}
- `ChecklistController` — GET /api/v1/applications/{id}/checklist, POST .../checklist/link
- `AdminController` — learner search/overview, document verify/reject, institution/programme management
- `AuditController` — GET /api/v1/audit (SuperAdmin only)

Security:
- `[Authorize]` everywhere; `[Authorize(Roles = "...")]` on admin/support endpoints
- Learners access only their own documents
- Every sensitive admin/support action writes an `AuditLogEntry`
- Document download streams only after ownership/role check — no direct file URLs

### 5. EF Core

Add new entities to `FundiLinkDbContext`. Generate migration `AddDocumentsAndAudit`.
Store enums as strings. Do NOT run migrations against a production database.

---

## Frontend: What to Build

### 1. Document Vault (`/documents`)
- `DocumentsPage.tsx` — upload (drag/drop or file picker), list documents with status badges (Pending=gray, Verified=green, Rejected=red + reason), download, delete (with confirmation)
- Client-side validation of type/size before upload, with clear error messages
- POPIA consent/notice text near upload

### 2. Application Checklist
- On `ApplicationDetailPage.tsx`, show the required-documents checklist with linked status and a control to attach an uploaded document

### 3. Admin Portal (`/admin`, role-gated routes)
- `AdminLearnersPage.tsx` — search learners, open overview
- `AdminLearnerDetailPage.tsx` — profile + applications + documents; verify/reject documents
- `AdminInstitutionsPage.tsx` / `AdminProgrammesPage.tsx` — CRUD for institutions and programmes
- `AuditLogPage.tsx` (SuperAdmin) — view audit entries
- Add a role-aware ProtectedRoute that checks the user's role claim

### 4. Routing & Tiles
- Add the routes above (protected, role-gated where applicable)
- Add a Documents tile to `ProfilePage.tsx`; show an Admin entry for staff roles

---

## Testing Requirements

**Backend:**
- Unit tests for upload validation (type/size reject), ownership enforcement on download/delete
- Unit tests for `VerifyDocumentCommand` writing an audit log entry
- Test that non-owner / wrong-role access is rejected (RBAC + POPIA)
- Test audit log append-only behaviour

**Frontend:**
- Document status badge mapping tests
- Upload validation error tests

---

## Security & Privacy Requirements

- No secrets committed; storage path is config, not a secret
- Documents never publicly accessible; all access authorized and ownership/role-checked
- Every sensitive admin/support action is audit-logged
- Right to erasure: deleting a document removes it from storage and soft-deletes the record
- Minors: surface guardian-consent document type and notice
- Never claim official government/university/NSFAS integration

---

## What NOT to Do

- Do not integrate a cloud storage provider yet (local disk for MVP); keep the interface clean for later
- Do not send real emails or SMS
- Do not run migrations against a production database
- Do not allow audit log edits or deletes
- Do not implement payment or bursary matching (Phase 5)

---

## Output Requirements

1. Confirm `dotnet build` passes
2. Confirm `dotnet test` passes
3. Confirm `npm run build` and `npm test -- --run` pass in `src/fundilink-web`
4. List all files created or modified
5. Confirm no secrets committed; flag all security-relevant changes
6. Commit: `Add Phase 4 document vault and admin portal`
7. Push to `claude/happy-dirac-n7qgtg`
8. Update `ROADMAP.md` Phase 4 checklist
9. Update this file (`NEXT_PROMPT.md`) with the Phase 5 prompt

---

## Definition of Done for Phase 4

- [ ] Learners can upload, list, download, and delete their own documents securely
- [ ] Upload validation (type/size) enforced server-side
- [ ] Application document checklist works
- [ ] Support/Admin can search learners and verify/reject documents (audit-logged)
- [ ] Admin can manage institutions and programmes (audit-logged)
- [ ] SuperAdmin can view the audit log; audit entries are append-only
- [ ] All RBAC and POPIA rules enforced
- [ ] Build and tests green
- [ ] Zero secrets committed
