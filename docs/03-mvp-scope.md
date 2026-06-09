# 03 — MVP Scope

## Title
FundiLink SmartApply — MVP Scope Definition

## Purpose
Clearly define what is in and out of scope for the MVP to prevent scope creep and keep the team focused.

---

## MVP Name
**FundiLink SmartApply**

## MVP Goal
Enable a South African learner to create one profile, calculate their APS, discover qualifying programmes, organise their documents, and track their applications — all in one place.

## In Scope

### Core Learner Flow
1. Register with email, verify account, agree to POPIA consent
2. Create education profile (personal info, school, grade)
3. Enter NSC subjects and results
4. View calculated APS score
5. Browse and filter programmes matched to their APS and subjects
6. Save programmes to application tracker
7. Upload and organise supporting documents
8. Track application status per institution
9. Receive deadline notifications (email + in-app)

### Admin Flow
1. Support Agent: search learners, view profiles (with audit log), add notes
2. Admin: manage institutions, programmes, users
3. SuperAdmin: audit logs, platform config, role management

### School Dashboard (foundational)
1. School admin account and school setup
2. View learners associated with the school
3. See aggregate progress metrics

### Technical
- ASP.NET Core Clean Architecture backend
- PostgreSQL database
- JWT authentication
- React + Vite + TypeScript frontend
- Mobile-first responsive design
- Email notifications (basic)
- Secure document storage (local for dev, cloud in staging/production)
- Swagger API documentation
- GitHub Actions CI/CD

## Out of Scope (MVP)

- WhatsApp / SMS integration
- AI guidance chatbot
- Automated application submission to institutions
- Bursary partner portal or integrations
- Student accommodation module
- Skills development / learnerships module
- Mentorship or early career features
- Payment processing
- Official NSFAS, CAO, or university API integration
- Native mobile app (iOS / Android)
- Multi-language support (English only)
- Advanced analytics dashboard
- Public API for third-party access

## MVP Success Criteria
- A learner can complete the full journey: register → profile → APS → match → document → track
- APS calculator is accurate for standard NSC results
- Programme matching returns relevant results
- Documents can be uploaded and stored securely
- Notifications are sent for deadlines
- Admin portal allows support agents to assist learners
- All RBAC rules enforced
- No secrets committed
- CI passes

## Next Actions
- Complete Phase 0 documentation (current)
- Begin Phase 1: solution scaffold
- Identify initial programme data sources for seed data
