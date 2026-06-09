# 13 — Feature Backlog

## Title
FundiLink — Feature Backlog

## Purpose
Track all planned, in-progress, and future features. Prioritised by phase and impact.

---

## How to Read This Backlog

| Column | Meaning |
|---|---|
| Feature | Short feature name |
| Phase | Which roadmap phase this belongs to |
| Priority | High / Medium / Low within the phase |
| Status | Planned / In Progress / Done / Deferred |
| Notes | Key requirements or constraints |

---

## Phase 1 — Solution Scaffold

| Feature | Priority | Status | Notes |
|---|---|---|---|
| .NET Clean Architecture solution | High | Planned | Domain, Application, Infrastructure, Api projects |
| React + Vite + TypeScript scaffold | High | Planned | Feature-folder structure |
| Docker Compose for PostgreSQL | High | Planned | Local dev only |
| EF Core + Identity setup | High | Planned | Initial migration |
| JWT auth baseline | High | Planned | Register, login, token refresh |
| Health check endpoint | Medium | Planned | `/health` |
| Swagger / OpenAPI | Medium | Planned | Dev environment only initially |
| GitHub Actions CI | High | Planned | Build + test on push |

---

## Phase 2 — Student Profile and APS

| Feature | Priority | Status | Notes |
|---|---|---|---|
| Learner registration | High | Planned | Email verification required |
| POPIA consent at registration | High | Planned | Non-negotiable |
| Guardian consent for under-18 | High | Planned | POPIA + Children's Act |
| Login and JWT refresh | High | Planned | Secure token rotation |
| Education profile CRUD | High | Planned | Personal + school info |
| NSC subject and mark entry | High | Planned | Subject selector from approved list |
| APS calculation | High | Planned | Standard NSC scale; handle LO |
| Profile completeness indicator | Medium | Planned | Encourage completion |
| Password reset flow | High | Planned | Email-based |
| Basic document upload | Medium | Planned | Local storage in dev |

---

## Phase 3 — Programme Matching and Application Tracker

| Feature | Priority | Status | Notes |
|---|---|---|---|
| Institution database | High | Planned | Seed from public data |
| Programme database | High | Planned | Seed from public data |
| APS + subject matching engine | High | Planned | Core feature |
| Programme search and filter | High | Planned | Type, province, field |
| Save programme to tracker | High | Planned | CTA on each programme |
| Application tracker | High | Planned | Status lifecycle |
| Document checklist per application | High | Planned | Per institution requirements |
| Document vault | High | Planned | Upload, status, organise |
| Deadline tracking | High | Planned | Visual indicator |
| In-app notifications | Medium | Planned | Notification bell |
| Email notifications | High | Planned | Deadline + status |

---

## Phase 4 — Admin Portal and School Dashboard

| Feature | Priority | Status | Notes |
|---|---|---|---|
| Support Agent: learner search | High | Planned | Audit logged |
| Support Agent: profile view | High | Planned | Consent + audit log |
| Support Agent: notes + flagging | Medium | Planned | Internal tool |
| Admin: institution management | High | Planned | CRUD |
| Admin: programme management | High | Planned | CRUD |
| Admin: user management | High | Planned | Role management |
| SuperAdmin: audit log viewer | High | Planned | Read-only |
| School admin registration | High | Planned | School setup flow |
| School dashboard: learner list | High | Planned | School-scoped view |
| School dashboard: progress metrics | Medium | Planned | Aggregate counts |

---

## Phase 5 — WhatsApp and Notifications

| Feature | Priority | Status | Notes |
|---|---|---|---|
| WhatsApp Business API | High | Planned | Requires Meta approval |
| WhatsApp notifications | High | Planned | Deadline + status |
| WhatsApp chatbot (basic) | Medium | Planned | Profile updates, FAQ |
| SMS fallback | Low | Planned | For no-WhatsApp users |
| Notification preferences | Medium | Planned | Learner chooses channels |

---

## Phase 6 — Bursary Hub

| Feature | Priority | Status | Notes |
|---|---|---|---|
| Bursary database | High | Planned | Public bursary listings |
| Bursary eligibility matching | High | Planned | APS + criteria |
| Bursary application tracker | High | Planned | Integrated with existing |
| Partner portal foundation | Medium | Planned | Self-service listing management |
| Skills/learnership module | Low | Planned | Foundation only |

---

## Phase 7 — AI Guidance

| Feature | Priority | Status | Notes |
|---|---|---|---|
| AI assistant | High | Planned | LLM integration |
| Contextual guidance | High | Planned | Based on learner profile |
| Programme Q&A | High | Planned | Hallucination-protected |
| Human escalation | High | Planned | Fallback to support agent |

---

## Deferred / Out of Scope (MVP)

| Feature | Notes |
|---|---|
| Native mobile app | Post-MVP; PWA first |
| Multi-language support | Post-MVP |
| Official NSFAS integration | Requires formal partnership |
| Official CAO integration | Requires formal partnership |
| Payment gateway | Phase 6+ |
| CV builder | Future feature |
| Mentorship matching | Future feature |

---

## Next Actions
- Use this backlog to drive Phase 1 development
- Review and reprioritise after pilot school feedback
- Add issue tracking links once GitHub Issues are set up
