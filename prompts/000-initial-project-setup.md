# Prompt 000 — Initial Project Setup

## Phase
Phase 0: Documentation and Planning Foundation

## Status
Complete

## Purpose
Create the complete project documentation foundation that will guide all future development prompts. This is the first setup task — no application code is created.

---

## Context
- New, empty GitHub repository for FundiLink by ZulTek
- No existing code, no existing files
- FundiLink is a South African student opportunity platform
- This is ZulTek's first flagship product

---

## The Prompt

> You are Claude Code working inside a new GitHub repository for a product called FundiLink by ZulTek.
>
> This is the first setup task. Do not build the full application yet. Your job is to create the complete project documentation foundation that will guide all future development prompts.
>
> **Product Context:**
> FundiLink is ZulTek's first flagship product.
> FundiLink is a South African student opportunity platform that helps learners, especially learners from rural and under-resourced areas, create one education profile, calculate APS, understand what they qualify for, prepare documents, track applications, and connect with study, funding, accommodation, skills, and early-career opportunities.
>
> **Public brand:**
> - FundiLink
> - Full name: FundiLink by ZulTek
> - MVP product: FundiLink SmartApply
> - Tagline: One profile. Every opportunity.
>
> [... full prompt as provided to Claude Code ...]

---

## Result

### Files Created

**Root level:**
- `CLAUDE.md` — AI agent instructions (permanent rules for all sessions)
- `README.md` — Project overview and structure
- `PROJECT_CONTEXT.md` — Full product context
- `PRODUCT_REQUIREMENTS.md` — MVP requirements with roles, features, exclusions
- `ARCHITECTURE.md` — Technical architecture (Clean Architecture, React, PostgreSQL)
- `SECURITY.md` — Security baseline (secrets, auth, RBAC, POPIA, audit logging)
- `ROADMAP.md` — 7-phase development roadmap
- `CONTRIBUTING.md` — Contribution guidelines
- `.gitignore` — Comprehensive ignore file (.NET, Node, secrets, OS files)

**docs/ folder (20 documents):**
- `01-product-vision.md`
- `02-business-plan.md`
- `03-mvp-scope.md`
- `04-technical-blueprint.md`
- `05-go-to-market.md`
- `06-pitch-one-pager.md`
- `07-privacy-compliance.md`
- `08-product-roadmap.md`
- `09-brand-identity.md`
- `10-landing-page-copy.md`
- `11-user-personas.md`
- `12-user-journey-map.md`
- `13-feature-backlog.md`
- `14-database-design.md`
- `15-api-specification.md`
- `16-ui-ux-screen-list.md`
- `17-school-pilot-proposal.md`
- `18-investor-partner-pitch-content.md`
- `19-pricing-strategy.md`
- `20-risk-register.md`

**prompts/ folder:**
- `prompts/README.md`
- `prompts/000-initial-project-setup.md` (this file)
- `prompts/NEXT_PROMPT.md`

### No application code created (by design)
### No packages installed (by design)
### No backend or frontend scaffolded (by design)
