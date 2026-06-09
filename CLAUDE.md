# CLAUDE.md — FundiLink by ZulTek

This file contains permanent instructions for all Claude Code sessions working on this repository.
Read this file completely before making any changes.

---

## Product Summary

**FundiLink** is ZulTek's flagship product — a South African student opportunity platform.
It helps learners (especially from rural and under-resourced areas) to:

- Create one unified education profile
- Calculate APS scores
- Understand what programmes and funding they qualify for
- Prepare and organise supporting documents
- Track applications to universities, TVETs, bursaries, and skills programmes
- Connect with accommodation, skills development, and early-career opportunities

**Full name:** FundiLink by ZulTek
**MVP product:** FundiLink SmartApply
**Tagline:** One profile. Every opportunity.

**IMPORTANT POSITIONING:** FundiLink is NOT an official government, university, TVET, NSFAS, or bursary admissions platform unless a formal partnership is documented and implemented. FundiLink helps learners prepare, organise, understand, and track applications. Final submissions happen through official institution or funding portals unless official integration exists and is documented.

---

## Tech Direction

### Backend
- **Runtime:** ASP.NET Core (.NET 8 or .NET 9)
- **Architecture:** Clean Architecture (Domain, Application, Infrastructure, API layers)
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core (preferred); Dapper for complex queries if needed
- **API style:** REST API
- **Auth:** ASP.NET Identity + JWT bearer tokens
- **Testing:** xUnit, FluentAssertions, Moq

### Frontend
- **Framework:** React + Vite + TypeScript
- **Styling:** Tailwind CSS (mobile-first)
- **State management:** React Context or Zustand (decide at scaffold stage)
- **HTTP client:** Axios or fetch with typed wrappers
- **Testing:** Vitest + React Testing Library

### Infrastructure
- **Containerisation:** Docker Compose for local development (PostgreSQL, API)
- **CI/CD:** GitHub Actions
- **Hosting:** TBD — Azure or local VPS for MVP

### Future Integrations (only after formal setup)
- WhatsApp Business API
- Email notifications (SendGrid or similar)
- Secure document storage (Azure Blob or S3-compatible)
- Payment gateway
- Institution/partner API integrations (only after formal partnership agreement)

---

## Coding Standards

- Follow Clean Architecture layer boundaries — no cross-layer leakage
- Use meaningful, descriptive names; no abbreviations except well-known ones (ID, DTO, API)
- One responsibility per class/component
- No magic strings — use constants or enums
- Validate all inputs at API boundary
- Return typed responses — never `object` or `dynamic` in public APIs
- Use DTOs for API layer; domain models stay in the domain layer
- Frontend: TypeScript strict mode; no `any` unless absolutely unavoidable
- Write tests for business logic and API endpoints; aim for coverage on critical paths
- Keep functions small; extract complexity into named methods
- No commented-out code committed to main
- No `TODO` comments left without a linked issue

---

## Security Rules

- **Never commit secrets, passwords, API keys, or connection strings**
- Use `dotnet user-secrets` for local .NET development
- Use environment variables for all deployment secrets
- Use placeholder values in `appsettings.json` (e.g., `"ConnectionStrings__Default": "SET_VIA_ENV"`)
- Never put real credentials in `.env`, `appsettings.Production.json`, or any doc file
- Enforce HTTPS in production; never allow HTTP for authenticated routes
- Use role-based access control (RBAC) on all API endpoints
- Sanitise and validate all user input — reject, do not sanitise-then-use for critical fields
- Protect uploaded documents — no public, unauthenticated access to document storage
- Add audit logs for all sensitive admin actions (role changes, data deletion, admin overrides)
- Use parameterised queries — never concatenate SQL
- Keep dependencies up to date; check for known CVEs before adding new packages

---

## POPIA / Privacy Rules

FundiLink processes:
- Personal information (name, ID, contact details)
- Academic records (grades, certificates)
- Identity documents
- Guardian/parent details
- Possibly minors' data (learners under 18)

Build with **privacy-by-design**:
- Collect only data that is necessary for the stated purpose
- Display clear consent notices at data collection points
- Require guardian consent where the learner is under 18
- Restrict data access by role — no role sees more than it needs
- Log all sensitive admin access and data actions
- Support data deletion requests (right to erasure)
- Never share or expose user data to third parties without consent and legal basis
- Do not store documents longer than necessary — define retention policies
- Document all data flows in `/docs/07-privacy-compliance.md`

---

## AI Agent Safety Rules

- Always inspect existing files before editing them
- Never overwrite a file that contains important content without reading it first
- Prefer small, safe, incremental changes over large rewrites
- After each task, explicitly list which files were created or modified
- Run build and tests whenever application code exists
- Update documentation when architecture or API contracts change
- If uncertain about a requirement, make a safe assumption and document it clearly
- Never invent integrations that do not exist in the codebase or agreements
- Never claim official government, university, NSFAS, or bursary integration unless it is implemented, tested, and documented
- Never delete migrations, audit logs, or user data without explicit instruction and confirmation
- Do not run destructive database commands without explicit instruction
- When making security-relevant changes, flag them explicitly in the commit message and summary

---

## Git Rules

- Branch: `claude/happy-dirac-n7qgtg` is the active development branch
- Never push directly to `main` without review
- Commit messages: imperative mood, present tense, clear and descriptive
  - Good: `Add APS calculator service and unit tests`
  - Bad: `fixed stuff` or `changes`
- Include the scope of change in the message where helpful
- One logical change per commit
- Never commit `.env`, secrets, or build output
- Tag releases with semantic versioning (`v0.1.0`, `v1.0.0`, etc.)

---

## Testing Rules

- Write unit tests for all domain logic and application services
- Write integration tests for API endpoints and database interactions
- Tests must pass before merging
- Do not disable or skip tests without a documented reason
- Use test data factories — never use real learner data in tests
- Test security: verify unauthorised access is rejected on all protected endpoints

---

## Documentation Rules

- Update `ARCHITECTURE.md` when the architecture changes
- Update `PRODUCT_REQUIREMENTS.md` when requirements change
- Update `/docs/15-api-specification.md` when API contracts change
- Update `ROADMAP.md` when the phase plan changes
- Keep `CLAUDE.md` current — if a rule is outdated, update it

---

## Definition of Done

A feature is done when:
1. Code is written and follows coding standards
2. Unit and integration tests are written and passing
3. Security and RBAC rules are applied
4. API documentation is updated
5. POPIA/privacy rules are respected
6. No secrets are committed
7. Code is reviewed (or self-reviewed against standards)
8. Documentation is updated if needed
9. Build passes in CI

---

## What Claude Must Do Before Making Changes

1. Read this file (`CLAUDE.md`)
2. Read `PRODUCT_REQUIREMENTS.md` for context
3. Read `ARCHITECTURE.md` for structural context
4. Read the specific file(s) that will be changed
5. Understand the current state before proposing changes
6. Flag any security or privacy concerns before proceeding

## What Claude Must Never Do

- Commit secrets, API keys, or real connection strings
- Claim official government/university/NSFAS integration that does not exist
- Build features not in the agreed scope without flagging it
- Skip tests to make a feature "work faster"
- Overwrite migrations without explicit instruction
- Delete user data or audit logs without explicit instruction and confirmation
- Use `any` types or disable TypeScript strict checks without a documented reason
- Push to `main` directly
- Install new packages without noting it in the summary
