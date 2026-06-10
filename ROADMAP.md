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

## Notes

- Phases may overlap or be reprioritised based on learner feedback and business needs
- Security and POPIA compliance are non-negotiable at every phase
- Each phase must have tests passing and documentation updated before the next phase begins
- See `/docs/08-product-roadmap.md` for a more detailed version of this roadmap
