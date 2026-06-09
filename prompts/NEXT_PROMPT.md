# NEXT_PROMPT.md — Phase 1: Solution Scaffold

## When to Run This Prompt
Run this prompt after Phase 0 documentation is complete and committed.
This prompt creates the working application scaffold that all future phases will build on.

---

## Prompt 001 — Solution Scaffold and Local Development Setup

---

Read `CLAUDE.md` before starting. Follow all rules in that file.

You are Claude Code working on FundiLink by ZulTek. Phase 0 documentation is complete. You are now starting Phase 1: creating the solution scaffold and local development setup.

**Goal:** Create the full working solution structure. No business logic yet — just the scaffold, configuration, and a running foundation.

**Context:**
- Product: FundiLink SmartApply (see `PRODUCT_REQUIREMENTS.md`)
- Architecture: Clean Architecture ASP.NET Core + React + Vite + TypeScript (see `ARCHITECTURE.md`)
- Database: PostgreSQL (see `docs/14-database-design.md`)
- No secrets should ever be committed

---

## What to Create

### 1. .NET Solution and Projects

Create the following solution structure:

```
src/
├── FundiLink.Domain/           (Class Library)
├── FundiLink.Application/      (Class Library)
├── FundiLink.Infrastructure/   (Class Library)
└── FundiLink.Api/              (ASP.NET Core Web API)

tests/
├── FundiLink.Domain.Tests/     (xUnit)
├── FundiLink.Application.Tests/(xUnit)
└── FundiLink.Api.Tests/        (xUnit + WebApplicationFactory)
```

Create a `FundiLink.sln` file at the root with all projects referenced.

**Project dependencies (Clean Architecture):**
- Domain: no dependencies on other FundiLink projects
- Application: depends on Domain only
- Infrastructure: depends on Application and Domain
- Api: depends on Application and Infrastructure

**Packages to install:**
- FundiLink.Domain: none (pure .NET)
- FundiLink.Application: MediatR, FluentValidation, FluentValidation.DependencyInjectionExtensions
- FundiLink.Infrastructure: Microsoft.EntityFrameworkCore, Npgsql.EntityFrameworkCore.PostgreSQL, Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Tools
- FundiLink.Api: Microsoft.AspNetCore.Authentication.JwtBearer, Swashbuckle.AspNetCore, Microsoft.EntityFrameworkCore.Design
- Test projects: xUnit, FluentAssertions, Moq, coverlet.collector, Microsoft.AspNetCore.Mvc.Testing (for Api tests)

### 2. ASP.NET Core API Configuration

In `FundiLink.Api`:
- Configure ASP.NET Identity
- Configure JWT Bearer authentication
- Configure Swagger / OpenAPI (dev environment only)
- Add CORS policy (allow localhost frontend in dev)
- Add health check endpoint: `GET /health`
- Add global exception handling middleware
- Configure EF Core with PostgreSQL connection string from environment/user-secrets (placeholder only — no real connection string committed)
- Add request logging middleware

`appsettings.json` should contain only placeholder values:
```json
{
  "ConnectionStrings": {
    "Default": "SET_VIA_USER_SECRETS_OR_ENV"
  },
  "JwtSettings": {
    "SecretKey": "SET_VIA_USER_SECRETS_OR_ENV",
    "Issuer": "FundiLink",
    "Audience": "FundiLink",
    "ExpiryMinutes": 60
  }
}
```

### 3. EF Core Setup

In `FundiLink.Infrastructure`:
- Create `FundiLinkDbContext` extending `IdentityDbContext`
- Configure it with the connection string from app settings
- Create the initial migration (this will be the identity schema migration)
- Do not run the migration yet — just create it

### 4. React + Vite + TypeScript Frontend

Create `fundilink-web/` inside `src/`:

```
src/fundilink-web/
├── src/
│   ├── api/
│   ├── components/
│   ├── features/
│   │   └── auth/
│   ├── hooks/
│   ├── layouts/
│   ├── pages/
│   │   ├── HomePage.tsx
│   │   └── NotFoundPage.tsx
│   ├── types/
│   ├── utils/
│   ├── App.tsx
│   └── main.tsx
├── public/
├── index.html
├── vite.config.ts
├── tailwind.config.ts
├── tsconfig.json
├── tsconfig.node.json
└── package.json
```

Install these packages:
- react, react-dom
- react-router-dom
- axios
- tailwindcss, @tailwindcss/forms, autoprefixer, postcss
- @types/react, @types/react-dom (devDependencies)
- typescript (devDependencies)
- vite, @vitejs/plugin-react (devDependencies)
- vitest, @testing-library/react, @testing-library/jest-dom (devDependencies)

Configure:
- TypeScript strict mode
- Tailwind CSS with PostCSS
- Vite with React plugin
- React Router with a basic route: `/` → `HomePage`

`HomePage.tsx` should render: "FundiLink — Coming Soon" (placeholder only).

### 5. Docker Compose for Local PostgreSQL

Create `docker/docker-compose.yml`:

```yaml
version: '3.9'
services:
  postgres:
    image: postgres:16-alpine
    container_name: fundilink-postgres
    ports:
      - "5432:5432"
    environment:
      POSTGRES_DB: fundilink_dev
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
    volumes:
      - fundilink-pgdata:/var/lib/postgresql/data

volumes:
  fundilink-pgdata:
```

Create a `docker/.env.example` file showing the required variables (with placeholder values — no real passwords):
```
POSTGRES_USER=fundilink_local
POSTGRES_PASSWORD=REPLACE_WITH_YOUR_LOCAL_PASSWORD
```

The actual `docker/.env` file must be in `.gitignore` and must NOT be committed.

### 6. GitHub Actions CI Pipeline

Create `.github/workflows/ci.yml`:
- Trigger on push and pull_request to `main` and `develop`
- Job 1: Build and test .NET solution (`dotnet build`, `dotnet test`)
- Job 2: Build and lint frontend (`npm install`, `npm run build`, `npm run lint` if lint script exists)
- Use GitHub Actions secrets for any required environment values — do not hardcode them

### 7. Update README.md

Update `README.md` with working local setup instructions:
- Prerequisites
- How to start Docker PostgreSQL
- How to set up dotnet user-secrets (with example key names, no real values)
- How to run the API (`dotnet run`)
- How to run the frontend (`npm run dev`)
- How to run tests

---

## What NOT to Do

- Do not create any learner profile, APS, or programme features (Phase 2)
- Do not create any domain entities beyond the initial scaffolding
- Do not implement any business logic
- Do not commit any real secrets, connection strings, or passwords
- Do not add packages beyond what is listed above without noting it
- Do not run database migrations against a real database — only generate the migration file

---

## Security Requirements

- `appsettings.json` must contain only placeholders
- `.env` files must not be committed
- `docker/.env` must be in `.gitignore`
- No real connection strings anywhere in committed files
- JWT secret key must be set via user-secrets or environment variable
- Note in README: "Never commit real secrets"

---

## Output Requirements

After completing this prompt:

1. Confirm the solution builds: `dotnet build` passes
2. Confirm frontend builds: `npm run build` passes
3. Confirm tests run: `dotnet test` passes (even with zero tests initially)
4. List every file and folder created or modified
5. Show the final directory structure
6. Confirm no secrets are committed
7. Commit all changes with message: `Add Phase 1 solution scaffold and local dev setup`
8. Push to `claude/happy-dirac-n7qgtg`
9. Update `ROADMAP.md` Phase 1 checklist items as appropriate
10. Update `prompts/NEXT_PROMPT.md` with the Phase 2 prompt

---

## Definition of Done for Phase 1

- [ ] .NET solution with 4 projects builds cleanly
- [ ] 3 test projects exist and `dotnet test` passes
- [ ] React + Vite + TypeScript frontend builds cleanly
- [ ] Docker Compose file created for local PostgreSQL
- [ ] EF Core initial migration generated
- [ ] Health check endpoint responds at `/health`
- [ ] Swagger available in dev at `/swagger`
- [ ] GitHub Actions CI workflow created
- [ ] README updated with local setup instructions
- [ ] Zero secrets committed
- [ ] All files committed and pushed to branch
