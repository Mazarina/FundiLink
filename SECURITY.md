# SECURITY.md — FundiLink Security Baseline

FundiLink processes sensitive personal information including identity documents, academic records, and possibly minors' data. Security and privacy are non-negotiable.

---

## Secrets Management

### Rules
- **Never commit secrets, passwords, API keys, or connection strings** to version control
- No real credentials in any `.env`, `appsettings.json`, `appsettings.Production.json`, or documentation file
- Use only placeholder values in committed config (e.g., `"SET_VIA_USER_SECRETS"` or `"SET_VIA_ENV"`)

### Local Development (.NET)
- Use `dotnet user-secrets` for all local secrets
- Run: `dotnet user-secrets set "ConnectionStrings:Default" "your-local-connection-string"`
- User secrets are stored in the OS user profile, never in the repository

### Staging and Production
- All secrets provided via environment variables or a secrets manager (e.g., Azure Key Vault)
- No secrets in deployment scripts or CI/CD pipeline logs
- CI/CD secrets stored in GitHub Actions secrets — never in code

---

## Authentication

- ASP.NET Identity for user account management
- JWT Bearer tokens for API authentication
- Access token expiry: 15–60 minutes (configurable)
- Refresh token rotation: refresh tokens are single-use; invalidated on use
- Refresh tokens stored hashed in the database, not in plain text
- Account lockout after repeated failed login attempts
- Email verification required before account activation
- Password requirements: minimum 8 characters, mixed case, number, special character

---

## Authorisation (RBAC)

Roles: `Student`, `SchoolAdmin`, `SupportAgent`, `Admin`, `SuperAdmin`

### Principles
- Apply role-based access control at the API controller level using `[Authorize(Roles = "...")]`
- Apply policy-based checks in the application layer for complex ownership rules
- A learner can only access their own profile, documents, and applications
- SupportAgent can only access a learner's profile with a logged consent action
- SchoolAdmin can only see aggregate data for their school; not individual documents without consent
- Admin cannot access SuperAdmin functions
- Enforce at both API and service layer — do not rely on UI alone

---

## Document Security

- Documents are stored with a server-generated, non-guessable identifier
- No public URL access to document files — all access is via authenticated API
- Access control checked on every document retrieval request
- File type validation on upload: allow only PDF, JPG, PNG
- File size limit: 5 MB per file (configurable)
- Virus/malware scanning in production before documents are made accessible
- Documents linked to the owning learner — cross-learner access rejected
- Deleted documents: soft-delete with audit log; physical deletion on confirmed data erasure request

---

## Audit Logging

All sensitive actions must be logged to the `AuditLogs` table:

| Action | Log Entry |
|---|---|
| Admin views a learner profile | Who, when, which learner |
| SupportAgent accesses documents | Who, when, which learner, which document |
| Role change | Who changed, new role, who made the change |
| Data deletion request | Who requested, what was deleted, when |
| Admin login | Who, when, from where |
| Failed login attempts | Account, timestamp, IP |
| Password reset | Account, timestamp |
| Document upload / deletion | Who, what, when |

Audit logs must be:
- Read-only for all roles except SuperAdmin
- Retained for a minimum of 3 years (POPIA and legal requirement review needed)
- Never deleted during normal operations

---

## Input Validation

- Validate all inputs at the API boundary using FluentValidation
- Reject requests that fail validation — do not attempt to sanitise-then-use for critical fields
- Use parameterised queries exclusively — EF Core handles this by default; never use raw string-concatenated SQL
- Validate file uploads: type, size, filename (no path traversal characters)
- Reject unexpected fields in API requests (use strict DTOs, not dynamic objects)
- Validate ID numbers using the Luhn algorithm check for South African ID numbers
- Encode all user-supplied content before rendering in the frontend

---

## POPIA (Protection of Personal Information Act)

FundiLink operates under South African law. POPIA compliance is required.

### Data Principles
- **Purpose limitation:** Collect only data necessary for the stated purpose
- **Consent:** Obtain and record explicit consent at registration
- **Minor consent:** Guardian consent required for learners under 18; record and verify
- **Access restriction:** Role-based data access; no over-sharing
- **Retention:** Define and enforce data retention periods
- **Deletion:** Support data deletion requests (right to erasure); log the action
- **Breach notification:** Have a documented process for notifying affected users and the Information Regulator within 72 hours of a known breach
- **Data sharing:** Do not share personal information with third parties without consent and legal basis

### Technical Implementation
- Consent recorded at registration with timestamp and version of T&Cs/privacy policy
- Guardian consent field on learner profile for under-18s
- Data deletion endpoint for learners (soft delete + queued physical deletion)
- Export endpoint for data portability (right of access)
- Sensitive data fields encrypted at rest where feasible
- Document `/docs/07-privacy-compliance.md` with all data flows and processing records

---

## Secure Development Workflow

1. No secrets in code or config — use secrets management
2. Run `dotnet build` and tests before committing
3. Review for OWASP Top 10 issues in new code
4. Keep dependencies up to date — check for known CVEs before adding packages
5. Use HTTPS in staging and production — redirect HTTP to HTTPS
6. Set appropriate CORS policy — only allow known frontend origins
7. Security headers: add Content-Security-Policy, X-Content-Type-Options, X-Frame-Options, Referrer-Policy in production
8. Never log sensitive information (passwords, tokens, ID numbers, full document content) to application logs

---

## AI Coding Agent Security Rules

When an AI agent (Claude Code or similar) works on this codebase:

- Never accept generated code that introduces hardcoded secrets
- Review generated authentication and authorisation code carefully — do not trust without verification
- Ensure generated migrations do not drop important columns without explicit review
- Do not allow AI to generate code that bypasses RBAC or audit logging
- AI must not claim or implement integrations with official portals unless explicitly instructed and verified
- AI must flag any security-relevant change in the commit message and session summary
