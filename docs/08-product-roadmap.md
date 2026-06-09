# 08 — Product Roadmap (Detailed)

## Title
FundiLink — Detailed Product Roadmap

## Purpose
Provide a detailed, phase-by-phase product roadmap with features, milestones, and success metrics.

---

## Roadmap Principles
- Learner value first — every phase must deliver something meaningful to learners
- Mobile-first at every phase
- Security and POPIA compliance non-negotiable
- No phase is "done" without tests and documentation

---

## Phase 0: Documentation and Planning Foundation
**Target:** Complete before any code is written
**Status:** In Progress

| Deliverable | Status |
|---|---|
| CLAUDE.md and project standards | Done |
| Product requirements | Done |
| Architecture plan | Done |
| Security baseline | Done |
| 20 planning documents | Done |
| Prompt log | Done |

---

## Phase 1: Solution Scaffold
**Target:** ~1–2 weeks
**Goal:** Working, deployable skeleton

| Feature | Notes |
|---|---|
| .NET Clean Architecture solution | Domain, Application, Infrastructure, Api |
| React + Vite + TypeScript frontend scaffold | Feature-folder structure |
| Docker Compose for local PostgreSQL | Dev environment |
| EF Core with initial migration | Identity schema |
| JWT authentication baseline | Register, login, token |
| Health check endpoint | `/health` |
| Swagger / OpenAPI | Auto-generated |
| GitHub Actions CI | Build + test |
| README setup instructions | Updated |

---

## Phase 2: Student Profile and APS
**Target:** ~3–4 weeks
**Goal:** Learner can register, build a profile, and calculate APS

| Feature | Notes |
|---|---|
| Learner registration with email verification | POPIA consent at registration |
| Login and JWT refresh | Secure token flow |
| Education profile creation and editing | Personal + academic info |
| NSC subject and result entry | Manual entry |
| APS calculator | Standard NSC scale |
| Profile completeness indicator | Encourage completion |
| Guardian consent flow for under-18 | POPIA requirement |
| Basic document upload | Local storage for dev |
| Unit tests for APS calculation | Critical path |

---

## Phase 3: Programme Matching and Application Tracker
**Target:** ~4–5 weeks
**Goal:** Learner can find what they qualify for and track applications

| Feature | Notes |
|---|---|
| Institution and programme seed data | Public data sources |
| Programme matching engine | APS + subject matching |
| Programme search and filter | By type, province, field |
| Application tracker | Add, update, view applications |
| Document checklist per application | Configurable |
| Document vault | Upload, organise, status |
| Application deadline tracking | Visual indicators |
| In-app notifications | Notification bell |
| Email notifications | Deadlines, status changes |

---

## Phase 4: Admin Portal and School Dashboard
**Target:** ~3–4 weeks
**Goal:** Staff can support learners; schools can see their learners

| Feature | Notes |
|---|---|
| Support Agent: learner search and profile view | Audit logged |
| Support Agent: notes and flagging | Internal tool |
| Admin: institution and programme management | CRUD |
| Admin: user management | Role assignment |
| Usage reports | Basic metrics |
| SuperAdmin: audit log viewer | Read-only |
| School admin registration | School setup |
| School dashboard: learner list | Aggregate view |
| School dashboard: progress metrics | Profiles, docs, applications |
| RBAC fully tested | Security milestone |

---

## Phase 5: WhatsApp and Enhanced Notifications
**Target:** TBD (requires Meta Business verification)

| Feature | Notes |
|---|---|
| WhatsApp Business API integration | Requires Meta approval |
| WhatsApp notifications | Deadline reminders, status updates |
| WhatsApp basic chatbot | Profile updates, FAQ |
| SMS fallback | For no-smartphone users |
| Notification preferences | Learner chooses channels |

---

## Phase 6: Bursary Hub and Partner Integrations
**Target:** TBD

| Feature | Notes |
|---|---|
| Bursary database | Public bursary listings |
| Bursary eligibility matching | APS + criteria |
| Bursary application tracker | Integrated with existing tracker |
| Partner portal (institutions) | Self-service listing management |
| Payment gateway | Premium features if applicable |
| Skills/learnership module | Foundation only |

---

## Phase 7: AI Guidance Assistant
**Target:** TBD

| Feature | Notes |
|---|---|
| AI assistant in learner workflow | LLM integration |
| Contextual guidance | Based on profile and APS |
| Programme and funding Q&A | Accurate, hallucination-protected |
| Human escalation | When AI can't help |
| Responsible AI policy | Documented and implemented |

---

## Success Metrics by Phase

| Phase | Key Metric |
|---|---|
| Phase 1 | CI pipeline green; zero secrets committed |
| Phase 2 | 100% APS calculation accuracy on test cases |
| Phase 3 | Learner completes full journey (register to application tracked) |
| Phase 4 | All RBAC rules pass security tests |
| Phase 5 | WhatsApp notification delivery rate > 95% |
| Phase 6 | Bursary matches relevant to learner APS |
| Phase 7 | AI guidance accuracy validated |

---

## Next Actions
- Complete Phase 0 (current)
- Run Phase 1 scaffold prompt (see `/prompts/NEXT_PROMPT.md`)
