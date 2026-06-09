# ARCHITECTURE.md — FundiLink Technical Architecture

## Recommended Architecture

FundiLink uses **Clean Architecture** on the backend and a **React SPA** on the frontend, communicating over a REST API.

```
┌─────────────────────────────────────────────┐
│              React Frontend (SPA)            │
│         Vite + TypeScript + Tailwind         │
└────────────────────┬────────────────────────┘
                     │ HTTPS / REST API
┌────────────────────▼────────────────────────┐
│              ASP.NET Core API                │
│           (Presentation Layer)               │
├─────────────────────────────────────────────┤
│           Application Layer                  │
│     (Use Cases, DTOs, Interfaces)            │
├─────────────────────────────────────────────┤
│             Domain Layer                     │
│     (Entities, Value Objects, Rules)         │
├─────────────────────────────────────────────┤
│          Infrastructure Layer                │
│  (EF Core, PostgreSQL, Storage, Email, Auth) │
└─────────────────────────────────────────────┘
```

---

## Backend Layers

### Domain Layer (`FundiLink.Domain`)
- Core entities: `Learner`, `AcademicProfile`, `Subject`, `Programme`, `Institution`, `Application`, `Document`, `Notification`
- Value objects: `ApsScore`, `NscResult`, `ContactInfo`, `Address`
- Domain events (future)
- No dependencies on external libraries except base .NET

### Application Layer (`FundiLink.Application`)
- Use cases / command and query handlers
- DTOs (Data Transfer Objects) for API communication
- Interfaces for infrastructure services (e.g., `IDocumentStorage`, `IEmailService`)
- Validation (FluentValidation)
- APS calculation logic lives here
- No direct database access — depends on abstractions

### Infrastructure Layer (`FundiLink.Infrastructure`)
- Entity Framework Core DbContext (`FundiLinkDbContext`)
- PostgreSQL database
- EF Core migrations
- Repository implementations
- Document storage provider (local filesystem for dev; cloud blob in production)
- Email service implementation
- Identity and JWT token generation
- External API clients (WhatsApp, etc. — added in later phases)

### API Layer (`FundiLink.Api`)
- ASP.NET Core controllers
- Middleware: exception handling, request logging, auth
- JWT authentication configuration
- Role-based authorisation attributes
- Swagger / OpenAPI documentation
- Health check endpoints

---

## Frontend Structure

```
fundilink-web/
├── src/
│   ├── api/           # Typed API client functions
│   ├── components/    # Shared reusable components
│   ├── features/      # Feature-based modules
│   │   ├── auth/
│   │   ├── profile/
│   │   ├── aps/
│   │   ├── programmes/
│   │   ├── documents/
│   │   ├── applications/
│   │   └── admin/
│   ├── hooks/         # Shared custom hooks
│   ├── layouts/       # Page layout components
│   ├── pages/         # Route-level page components
│   ├── store/         # Global state (Zustand or Context)
│   ├── types/         # Shared TypeScript types
│   └── utils/         # Shared utility functions
├── public/
├── index.html
├── vite.config.ts
├── tailwind.config.ts
└── tsconfig.json
```

---

## Database Overview

**Engine:** PostgreSQL
**ORM:** Entity Framework Core with code-first migrations

### Core Tables (planned)
| Table | Purpose |
|---|---|
| `Users` | ASP.NET Identity users |
| `Learners` | Learner profiles linked to users |
| `AcademicProfiles` | NSC results and academic history |
| `NscSubjectResults` | Individual subject results per learner |
| `Institutions` | University, TVET, private college records |
| `Programmes` | Programme database (degree, diploma, etc.) |
| `ProgrammeRequirements` | APS and subject requirements per programme |
| `Applications` | Learner applications to programmes |
| `Documents` | Document metadata (not the file itself) |
| `DocumentFiles` | File storage references |
| `Notifications` | Notification records |
| `Schools` | Partner school records |
| `AuditLogs` | Sensitive action audit trail |

See `/docs/14-database-design.md` for the full schema design.

---

## API Overview

**Style:** REST
**Auth:** JWT Bearer tokens
**Versioning:** URL-based (`/api/v1/...`)
**Format:** JSON
**Documentation:** Swagger / OpenAPI (auto-generated)

### Base URL Pattern
```
/api/v1/{resource}
```

### Core Resource Groups
- `/api/v1/auth` — registration, login, token refresh
- `/api/v1/learners` — learner profile CRUD
- `/api/v1/academic-profile` — NSC results, APS
- `/api/v1/programmes` — programme search and matching
- `/api/v1/documents` — document upload and management
- `/api/v1/applications` — application tracker
- `/api/v1/notifications` — notification management
- `/api/v1/admin` — admin operations
- `/api/v1/schools` — school dashboard

See `/docs/15-api-specification.md` for the full specification.

---

## Security Architecture

- JWT tokens with configurable expiry
- Refresh token rotation
- Role-based authorisation at controller and service level
- HTTPS enforced in staging and production
- CORS policy: allow only known frontend origins
- Rate limiting on auth endpoints
- File upload validation (type, size, virus scan in production)
- Parameterised queries via EF Core (no raw SQL string concatenation)
- Sensitive data encrypted at rest (document storage)
- Audit log for admin and sensitive actions
- User-secrets for local dev; environment variables for deployments

---

## Deployment Environments

| Environment | Purpose | Database |
|---|---|---|
| Local Dev | Developer machine | Docker PostgreSQL |
| Staging | Internal testing | Staging PostgreSQL |
| Production | Live platform | Production PostgreSQL (managed) |

### Local Docker Compose
- PostgreSQL container
- API container (or run locally with `dotnet run`)
- Frontend dev server (`npm run dev`)

---

## Future Integrations (Not in MVP)

| Integration | Phase | Notes |
|---|---|---|
| WhatsApp Business API | Phase 5 | Requires Meta Business verification |
| SendGrid / Email provider | Phase 2 | Basic email in MVP |
| Azure Blob / S3 document storage | Phase 2 | Local filesystem for dev |
| Payment gateway | Phase 6+ | Bursary and premium features |
| NSFAS API | Phase 6+ | Only if official partnership |
| CAO / university API | Phase 6+ | Only if official partnership |
| AI guidance assistant | Phase 7 | LLM integration |
