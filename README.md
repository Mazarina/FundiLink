# FundiLink by ZulTek

> **One profile. Every opportunity.**

FundiLink is a South African student opportunity platform that helps learners — especially those from rural and under-resourced communities — navigate the complex landscape of higher education, funding, and skills development.

---

## What Is FundiLink?

Many South African learners qualify for university, TVET college, bursaries, and skills programmes, but never access them because the process is fragmented, confusing, and inaccessible. FundiLink solves this by giving every learner one intelligent profile that works across multiple opportunity types.

**FundiLink is not an official government or institution admissions portal.** It helps learners prepare, organise, and track their applications. Final submissions occur through official portals unless a formal integration partnership exists.

---

## MVP: FundiLink SmartApply

The MVP focuses on the core student application workflow:

| Feature | Description |
|---|---|
| Education Profile | One profile capturing personal, academic, and contact information |
| APS Calculator | Automatic APS score calculation from NSC results |
| Programme Matching | Match learner APS and subjects to qualifying programmes |
| Document Vault | Checklist and secure storage for required application documents |
| Application Tracker | Track application status across multiple institutions |
| Notifications | Email and in-app alerts for deadlines and status changes |
| Admin Portal | Support agent and admin management tools |
| School Dashboard | Basic school-level view for pilot schools |

---

## Target Users

- **Students/Learners** — Grade 11, Grade 12, and recent matriculants
- **School Administrators** — Teachers and counsellors supporting learners
- **Support Agents** — FundiLink staff assisting learners remotely
- **Admins / SuperAdmins** — ZulTek operations and platform management

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core (.NET 8/9), Clean Architecture |
| Database | PostgreSQL, Entity Framework Core |
| API | REST, JWT Authentication |
| Frontend | React + Vite + TypeScript, Tailwind CSS |
| Containerisation | Docker Compose (local dev) |
| CI/CD | GitHub Actions |

---

## Repository Structure

```
/
├── CLAUDE.md                  # AI agent instructions (read first)
├── README.md                  # This file
├── PROJECT_CONTEXT.md         # Full product context
├── PRODUCT_REQUIREMENTS.md    # MVP requirements
├── ARCHITECTURE.md            # Technical architecture
├── SECURITY.md                # Security baseline
├── ROADMAP.md                 # Phased development plan
├── CONTRIBUTING.md            # Contribution guidelines
├── .gitignore
├── docs/                      # Detailed planning documents
│   ├── 01-product-vision.md
│   ├── 02-business-plan.md
│   ├── ... (20 documents)
├── prompts/                   # Claude Code prompts log
│   ├── README.md
│   ├── 000-initial-project-setup.md
│   └── NEXT_PROMPT.md
├── src/                       # Application source (created in next phase)
│   ├── FundiLink.Api/
│   ├── FundiLink.Application/
│   ├── FundiLink.Domain/
│   ├── FundiLink.Infrastructure/
│   └── fundilink-web/
└── tests/                     # Test projects (created in next phase)
```

---

## Local Setup

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Node.js 20+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for local PostgreSQL)
- A PostgreSQL client (optional — e.g., pgAdmin, DBeaver)

### 1. Start Local PostgreSQL

```bash
cp docker/.env.example docker/.env
# Edit docker/.env and set a local password (never commit this file)
docker compose -f docker/docker-compose.yml --env-file docker/.env up -d
```

### 2. Configure .NET User Secrets (never commit real values)

```bash
cd src/FundiLink.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=fundilink_dev;Username=fundilink_local;Password=YOUR_LOCAL_PASSWORD"
dotnet user-secrets set "JwtSettings:SecretKey" "YOUR_LOCAL_JWT_SECRET_MIN_32_CHARS"
```

### 3. Apply Database Migrations

```bash
# From the repo root
export DOTNET_ROOT=/usr/local/dotnet  # adjust to your .NET install path
dotnet ef database update \
  --project src/FundiLink.Infrastructure \
  --startup-project src/FundiLink.Api
```

### 4. Run the API

```bash
cd src/FundiLink.Api
dotnet run
# API available at http://localhost:5000
# Swagger at http://localhost:5000/swagger
# Health check at http://localhost:5000/health
```

### 5. Run the Frontend

```bash
cd src/fundilink-web
npm install
npm run dev
# Frontend at http://localhost:5173
```

### 6. Run Tests

```bash
# Backend tests
dotnet test

# Frontend tests
cd src/fundilink-web && npm test
```

> **Security reminder:** Never commit `docker/.env`, user secrets, real connection strings, or JWT keys.

---

## Security

- No secrets, API keys, or real connection strings are stored in this repository
- Use `dotnet user-secrets` for local development
- See `SECURITY.md` for the full security baseline

---

## Project Status

**Current phase:** Phase 0 — Documentation and Planning Foundation

The application code has not been created yet. This repository currently contains only documentation, planning files, and project standards.

---

## About ZulTek

ZulTek is the company behind FundiLink. FundiLink is ZulTek's first flagship product, built to create real access to opportunity for South African learners.

---

## Licence

Proprietary — ZulTek. All rights reserved. Not open source at this stage.
