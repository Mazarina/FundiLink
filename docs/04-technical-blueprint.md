# 04 — Technical Blueprint

## Title
FundiLink — Technical Blueprint

## Purpose
Provide a detailed technical blueprint for developers and AI agents building the system.

---

## Solution Structure

```
FundiLink/
├── src/
│   ├── FundiLink.Domain/           # Core business entities and rules
│   ├── FundiLink.Application/      # Use cases, DTOs, interfaces
│   ├── FundiLink.Infrastructure/   # EF Core, PostgreSQL, storage, email
│   ├── FundiLink.Api/              # ASP.NET Core REST API
│   └── fundilink-web/              # React + Vite + TypeScript frontend
├── tests/
│   ├── FundiLink.Domain.Tests/
│   ├── FundiLink.Application.Tests/
│   ├── FundiLink.Infrastructure.Tests/
│   ├── FundiLink.Api.Tests/
│   └── FundiLink.Web.Tests/
├── docker/
│   └── docker-compose.yml
└── docs/
```

## Backend: Key Technical Decisions

### ASP.NET Core + Clean Architecture
- Domain layer: pure C#, no framework dependencies
- Application layer: MediatR (CQRS pattern) or plain services — decide at scaffold
- Infrastructure: EF Core with migrations, ASP.NET Identity
- API: Minimal API or Controllers — Controllers preferred for RBAC clarity

### Database
- PostgreSQL via EF Core
- Code-first migrations
- Soft deletes for audit compliance
- Audit log table for sensitive actions
- No stored procedures in MVP (use EF Core queries)

### Authentication / Authorisation
- ASP.NET Identity for user management
- JWT Bearer tokens (access + refresh)
- Role-based policies: Student, SchoolAdmin, SupportAgent, Admin, SuperAdmin

### Document Storage
- Local filesystem in development
- Azure Blob Storage or S3-compatible in production
- Files referenced by metadata in DB, not stored in DB
- Access controlled via authenticated API endpoint

### APS Calculation Engine
- Deterministic function in the Application layer
- Inputs: list of subjects with percentage scores
- Output: APS score + breakdown
- Handle Life Orientation (not counted at most universities)
- Configurable per institution if needed in future

## Frontend: Key Technical Decisions

### React + Vite + TypeScript
- Strict TypeScript mode
- Feature-folder structure (`/features/auth`, `/features/profile`, etc.)
- Tailwind CSS with custom FundiLink brand tokens
- Axios for API calls with typed response wrappers
- React Router for navigation
- Zustand or React Context for global state (decide at scaffold)
- React Hook Form + Zod for form validation
- Vitest + React Testing Library for unit tests

### Mobile-First Design
- Tailwind responsive utilities (mobile default, `md:` and `lg:` for larger screens)
- Minimum touch target size: 44px
- Optimised for low-bandwidth environments (lazy loading, minimal bundle size)

## Infrastructure

### Docker Compose (local dev)
```yaml
services:
  postgres:
    image: postgres:16
    ports: ["5432:5432"]
    environment:
      POSTGRES_DB: fundilink_dev
      POSTGRES_USER: SET_VIA_ENV
      POSTGRES_PASSWORD: SET_VIA_ENV
```

### GitHub Actions CI
- Trigger: push and PR to `main` and `develop`
- Jobs: build backend, run tests, build frontend, lint
- Secrets: stored in GitHub Actions secrets

## API Design Conventions

- URL: `/api/v1/{resource}`
- Methods: GET (read), POST (create), PUT (replace), PATCH (update), DELETE (remove)
- Responses: JSON, consistent envelope `{ data, errors, meta }`
- Errors: RFC 7807 Problem Details format
- Pagination: `?page=1&pageSize=20` with `meta.totalCount` in response
- Authentication: `Authorization: Bearer <token>` header

## Next Actions
- Create solution scaffold (Phase 1)
- Set up Docker Compose for local PostgreSQL
- Configure EF Core with initial migration
- Set up GitHub Actions CI pipeline
