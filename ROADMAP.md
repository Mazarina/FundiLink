# ROADMAP.md — FundiLink Development Roadmap

## Overview

FundiLink is built in phases. Each phase builds on the last. No phase begins until the previous phase is stable, tested, and documented.

---

## Phase 0: Documentation and Planning Foundation
**Status:** In Progress
**Goal:** Create the complete project documentation, standards, and planning foundation before any code is written.

### Deliverables
- [x] CLAUDE.md — AI agent instructions
- [x] README.md — Project overview
- [x] PROJECT_CONTEXT.md — Product context
- [x] PRODUCT_REQUIREMENTS.md — MVP requirements
- [x] ARCHITECTURE.md — Technical architecture
- [x] SECURITY.md — Security baseline
- [x] ROADMAP.md — This file
- [x] CONTRIBUTING.md — Contribution guidelines
- [x] .gitignore — Complete ignore file
- [x] /docs/ — 20 planning documents
- [x] /prompts/ — Prompt log and next prompt

---

## Phase 1: Solution Scaffold and Local Development Setup
**Status:** Complete
**Goal:** Create the working solution structure that future phases will build on.

### Deliverables
- [x] ASP.NET Core solution with Clean Architecture projects
  - FundiLink.Domain
  - FundiLink.Application
  - FundiLink.Infrastructure
  - FundiLink.Api
- [x] React + Vite + TypeScript frontend scaffold
- [x] PostgreSQL Docker Compose for local development
- [x] EF Core setup with initial migration (InitialIdentitySchema)
- [x] ASP.NET Identity configured
- [x] JWT authentication baseline
- [x] Basic health check endpoint (`/health`)
- [x] Swagger / OpenAPI configured
- [x] GitHub Actions CI pipeline (build + test)
- [x] Local setup documentation in README

---

## Phase 2: Student Profile and Academic Record
**Status:** Complete
**Goal:** A learner can register, create a profile, and enter their academic results.

### Deliverables
- [x] Learner registration with email verification
- [x] Login and JWT token management
- [x] Learner profile creation and editing
- [x] Academic profile: enter NSC subjects and results
- [x] APS calculator (backend logic + frontend display)
- [x] Basic frontend: registration, login, profile pages
- [x] POPIA consent at registration
- [x] Guardian consent field for under-18s
- [x] Unit tests for APS calculation (28 Application tests, 3 Domain tests)
- [x] API integration tests for auth and profile endpoints (1 API test)
- [x] Frontend component tests (7 Vitest tests)

---

## Phase 3: Programme Matching and Application Tracker
**Status:** Complete
**Goal:** A learner can see what they qualify for and start tracking applications.

### Deliverables
- [x] Institution and programme database seed data (guidance only, disclaimed)
- [x] Programme matching engine (APS + subject matching)
- [x] Programme search and filter UI
- [x] Application tracker: create, update, view, delete applications
- [x] Application deadline tracking (deadline date per application)
- [x] Unit tests for matching engine and create-application handler
- [x] Frontend component tests (ProgrammeCard, ApplicationStatusBadge)
- [ ] Document checklist per application (moved to Phase 4 — document vault)
- [ ] Document vault with per-document status (moved to Phase 4)
- [ ] Basic in-app notifications (deferred)
- [ ] Email notifications (deadline alerts, status changes) (deferred)

---

## Phase 4: Document Vault and Admin Portal
**Status:** Complete
**Goal:** FundiLink staff can support learners; learners can manage supporting documents.

### Deliverables
- [x] Learner document vault (upload, list, download, delete) with per-document status
- [x] Document checklist per application with document linking
- [x] Secure local-disk document storage with path-traversal protection
- [x] Support Agent: learner search and overview (with audit log)
- [x] Support Agent/Admin: document verification and rejection
- [x] Admin: institution and programme data management (create/update)
- [x] SuperAdmin: audit log viewer
- [x] RBAC enforced on all admin/support endpoints
- [x] Append-only audit logging for all sensitive admin actions
- [x] Admin portal UI (learner search, learner detail, audit log)
- [ ] Support Agent: notes and flagging (deferred)
- [ ] Admin: user management (deferred)
- [ ] Admin: usage reports (deferred)
- [ ] School admin registration and school dashboard (deferred)

---

## Phase 5: WhatsApp and Enhanced Notifications
**Status:** Complete
**Goal:** Meet learners where they are — on WhatsApp.

### Deliverables
- [x] Multi-channel notification service architecture (Email, WhatsApp, SMS) behind interfaces
- [x] Notification preferences management for learners (Email opt-out, WhatsApp/SMS opt-in)
- [x] Append-only notification log (delivery audit trail)
- [x] Notification triggers wired to registration, application status change, and document verify/reject
- [x] WhatsApp notification channel (stubbed — no Meta credentials; logs only)
- [x] SMS fallback channel (stubbed — no gateway credentials; logs only)
- [x] Notification preferences UI with honest "coming soon" labelling for WhatsApp/SMS
- [ ] WhatsApp Business API integration (requires Meta Business verification — deferred)
- [ ] WhatsApp basic chatbot for profile updates and deadline reminders (deferred)

---

## Phase 6: Bursary Hub and Partner Integrations
**Status:** Complete (MVP)
**Goal:** Expand beyond university applications to bursaries and funding.

### Deliverables
- [x] Bursary database (curated public bursary information, guidance-only, disclaimed)
- [x] Bursary eligibility matching (APS + province against learner profile; "you may qualify" guidance)
- [x] Bursary application tracker (owner-scoped create / list / update status / delete)
- [x] Admin bursary CRUD (RBAC Admin/SuperAdmin, append-only audit logging)
- [x] Bursary Hub frontend (browse, detail, matches, tracker pages with guidance-only disclaimers)
- [x] Unit tests for matching, application handlers, admin audit logging, and domain; frontend page tests
- [ ] Partner portal foundation (read-only Bursary entity + admin CRUD delivered; no partner onboarding flow) (deferred)
- [ ] Payment gateway (for premium features if applicable) (deferred — no third-party integrations this phase)
- [ ] Skills development / learnership module foundation (deferred)
- [ ] Official integration framework (for formal partnerships only) (deferred)

---

## Phase 7: AI Guidance Assistant (MVP delivered)
**Status:** MVP delivered
**Goal:** Give every learner access to intelligent, personalised guidance.

### Deliverables
- [x] AI assistant integrated into learner workflow (`Ask FundiLink` page + profile tile)
- [x] Contextual guidance based on learner profile and APS (grounded strictly in the learner's own FundiLink data)
- [x] Answer a constrained set of questions about programmes, APS, documents, and funding
- [x] Plain language explanations
- [x] Safe, accurate responses — no hallucinated institution facts (deterministic `RuleBasedAiAssistantService` behind `IAiAssistantService`; no external LLM call)
- [x] Guidance-only disclaimer on every response + link to human support agent
- [x] Owner-scoped access and append-only, POPIA-minimal interaction log

### Deferred (post-MVP)
- [ ] Real LLM provider wired behind `IAiAssistantService` (API key via environment only)
- [ ] Free-form open-domain chat
- [ ] Automated human-handoff / live support agent routing
- [ ] Responsible AI policy documented in full

---

## Phase 8: Accommodation & Early-Career Opportunities (MVP delivered)
**Status:** MVP delivered
**Goal:** Connect learners with student accommodation and early-career opportunities.

### Deliverables
- [x] Student accommodation listings (curated public information, guidance-only, disclaimed) — browse/filter by province, near-institution, and accommodation type
- [x] Accommodation "may suit you" matching grounded in the learner's province/institution (guidance-only)
- [x] Owner-scoped accommodation interest tracking (track / list / update status)
- [x] Early-career opportunities (learnerships, internships, skills programmes, apprenticeships, entry-level jobs) — browse/filter by field, province, and opportunity type
- [x] Career eligibility hints from the learner's grade level / field (guidance-only)
- [x] Owner-scoped career interest tracking (track / list / update status)
- [x] Curated/seeded data behind repositories; no fabricated provider/employer facts; deterministic stub behind interfaces
- [x] `AccommodationController` and `CareerController` under `api/v1/...`, `[Authorize]`, owner-scoped, input validation at boundary
- [x] Frontend: accommodation & career browse pages, "my saved"/"my tracked" pages, guidance-only disclaimer banners, profile tiles, ProtectedRoute wiring
- [x] Backend unit tests (filtering, profile-grounded matches, owner-scope, interest tracking) and frontend page tests

### Deferred (post-MVP)
- [ ] Real external listing/job-board provider integration (behind existing interfaces; API key via environment only) (deferred)
- [ ] Bookings and payments for accommodation (deferred — no third-party integrations this phase)
- [ ] In-app application submission for career opportunities (applications happen on the provider's official channel) (deferred)

---

## Phase 9: Guardian Consent & Co-Access (MVP delivered)
**Status:** MVP delivered
**Goal:** Make guardian consent explicit, recorded, and auditable for minor learners (POPIA), and give consented guardians a minimised read-only co-access view.

### Deliverables
- [x] `GuardianConsent` entity — append-only consent history (grant + revoke records), recorded guardian identity, consent type/scope/timestamps; never mutated or deleted
- [x] `GuardianLink` entity — links a guardian user to a minor learner; the link alone grants nothing
- [x] `IConsentCheckService` — deterministic consent check from the latest record; no external identity-verification / e-signature provider (stub behind interface; future key via env only)
- [x] `Features/Consent/` CQRS — record consent, revoke consent (right to withdraw), consent state, consent history, link guardian, guardian-scoped read view, list linked learners (typed DTOs only)
- [x] Guardian co-access is consent-gated and data-minimised — a guardian sees only the consented scope (basic profile, or profile + application summaries); never the ID number, documents, or learner contact details
- [x] `ConsentController` under `api/v1/consent`, `[Authorize]`, owner/guardian-scoped, input validation at boundary
- [x] All consent grants/revocations, guardian links, and guardian access are append-only audit-logged
- [x] Frontend: `src/features/consent/` api wrappers + consent banner/badges, `ConsentPage` (manage consent), `GuardianViewPage` (minimised read-only view), profile tiles, ProtectedRoute wiring
- [x] Backend unit tests (record, revoke updates state, revoke without consent rejected, guardian access blocked without link/consent, scoped minimised view with consent, audit-log written) and frontend page tests (renders state, grant action triggers API, guardian view renders minimised data)

### Deferred (post-MVP)
- [ ] Real identity-verification / e-signature provider integration (behind `IConsentCheckService`; API key via environment only) (deferred)
- [ ] Guardian self-service account onboarding / invitation flow (deferred)
- [ ] Configurable, granular field-level co-access scopes (deferred)

---

## Phase 10: Data Subject Rights — Export & Erasure (MVP delivered)
**Status:** MVP delivered
**Goal:** Implement the POPIA right of access (data export) and right to erasure (erasure request + audited admin fulfilment), with append-only retention of audit and consent records as proof of lawful processing.

### Deliverables
- [x] `ErasureRequest` entity — tracked status lifecycle (Requested → Approved/Rejected → Fulfilled), recorded requester/reviewer identity and timestamps; EF config, DbSet, migration `AddErasureRequests`
- [x] `Learner.Anonymise()` — redacts personal/contact fields and soft-deletes the profile (tombstone) for erasure fulfilment
- [x] `IErasureService` / `DeterministicErasureService` — deterministic in-process fulfilment that anonymises/soft-deletes personal data (profile, academic profile, applications, bursary applications, document metadata, interests) and NEVER touches append-only audit or consent records; no external storage/email/delivery provider (future delivery channel behind the same interface; key via env only)
- [x] `Features/DataRights/` CQRS — export my data (typed owner-scoped DTO), request erasure, query my requests; admin: list pending, approve/reject (review), fulfil erasure (typed DTOs only)
- [x] `DataRightsController` under `api/v1/data-rights` — learner endpoints owner-scoped; admin endpoints RBAC-gated (review SupportAgent/Admin/SuperAdmin; fulfil Admin/SuperAdmin), `[Authorize]`, input validation at boundary
- [x] All export generation and all erasure request/approve/reject/fulfil actions are append-only audit-logged
- [x] Frontend: `src/features/data-rights/` api wrappers, `DataRightsPage` (download export, request erasure, see request status), `AdminErasureQueuePage` (review/fulfil queue), profile tiles, ProtectedRoute/role-gated routes in `App.tsx`
- [x] Backend tests (export returns owner-scoped data + audit, erasure request created + audit, duplicate-open request rejected, admin fulfil anonymises + marks fulfilled + audit, anonymise redacts/soft-deletes, erasure service preserves append-only audit/consent records) and frontend tests (renders request state, export triggers API, admin fulfil triggers API)

### Deferred (post-MVP)
- [ ] Real export delivery channel (secure download link / email) behind `IErasureService` (key via environment only) (deferred)
- [ ] Configurable data-retention schedules and automated retention enforcement (deferred)
- [ ] Learner-facing in-app rendering of the full export (currently downloaded as JSON) (deferred)

---

## Notes

- Phases may overlap or be reprioritised based on learner feedback and business needs
- Security and POPIA compliance are non-negotiable at every phase
- Each phase must have tests passing and documentation updated before the next phase begins
- See `/docs/08-product-roadmap.md` for a more detailed version of this roadmap
