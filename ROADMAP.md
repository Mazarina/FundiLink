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
**Status:** Planned
**Goal:** A learner can register, create a profile, and enter their academic results.

### Deliverables
- [ ] Learner registration with email verification
- [ ] Login and JWT token management
- [ ] Learner profile creation and editing
- [ ] Academic profile: enter NSC subjects and results
- [ ] APS calculator (backend logic + frontend display)
- [ ] Basic frontend: registration, login, profile pages
- [ ] Document upload baseline (local storage for dev)
- [ ] POPIA consent at registration
- [ ] Guardian consent field for under-18s
- [ ] Unit tests for APS calculation
- [ ] API integration tests for auth and profile endpoints

---

## Phase 3: Programme Matching and Application Tracker
**Status:** Planned
**Goal:** A learner can see what they qualify for and start tracking applications.

### Deliverables
- [ ] Institution and programme database seed data
- [ ] Programme matching engine (APS + subject matching)
- [ ] Programme search and filter UI
- [ ] Application tracker: create, update, view applications
- [ ] Document checklist per application
- [ ] Document vault with per-document status
- [ ] Application deadline tracking
- [ ] Basic in-app notifications
- [ ] Email notifications (deadline alerts, status changes)
- [ ] Unit tests for matching engine
- [ ] API and UI integration tests

---

## Phase 4: Admin Portal and School Dashboard
**Status:** Planned
**Goal:** FundiLink staff can support learners; school admins can see their learners.

### Deliverables
- [ ] Support Agent: learner search and profile view (with audit log)
- [ ] Support Agent: notes and flagging
- [ ] Admin: institution and programme data management
- [ ] Admin: user management
- [ ] Admin: usage reports
- [ ] SuperAdmin: audit log viewer
- [ ] School admin registration and school setup
- [ ] School dashboard: learner list and progress overview
- [ ] RBAC fully enforced and tested
- [ ] Audit logging for all sensitive admin actions
- [ ] Admin portal UI

---

## Phase 5: WhatsApp and Enhanced Notifications
**Status:** Planned
**Goal:** Meet learners where they are — on WhatsApp.

### Deliverables
- [ ] WhatsApp Business API integration (requires Meta Business verification)
- [ ] WhatsApp notification channel
- [ ] WhatsApp basic chatbot for profile updates and deadline reminders
- [ ] SMS fallback option
- [ ] Notification preferences management for learners
- [ ] Multi-channel notification service architecture

---

## Phase 6: Bursary Hub and Partner Integrations
**Status:** Planned
**Goal:** Expand beyond university applications to bursaries and funding.

### Deliverables
- [ ] Bursary database (public bursary information)
- [ ] Bursary eligibility matching
- [ ] Bursary application tracker
- [ ] Partner portal foundation (for institutions and funders to opt in)
- [ ] Payment gateway (for premium features if applicable)
- [ ] Skills development / learnership module foundation
- [ ] Official integration framework (for formal partnerships only)

---

## Phase 7: AI Guidance Assistant
**Status:** Planned
**Goal:** Give every learner access to intelligent, personalised guidance.

### Deliverables
- [ ] AI assistant integrated into learner workflow
- [ ] Contextual guidance based on learner profile and APS
- [ ] Answer questions about programmes, institutions, and funding
- [ ] Plain language explanations
- [ ] Safe, accurate responses — no hallucinated institution facts
- [ ] Fallback to human support agent when needed
- [ ] Responsible AI policy documented and implemented

---

## Notes

- Phases may overlap or be reprioritised based on learner feedback and business needs
- Security and POPIA compliance are non-negotiable at every phase
- Each phase must have tests passing and documentation updated before the next phase begins
- See `/docs/08-product-roadmap.md` for a more detailed version of this roadmap
