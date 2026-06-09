# CONTRIBUTING.md — FundiLink

## Who Can Contribute

FundiLink is a proprietary ZulTek product. Contributions are made by the ZulTek development team and authorised collaborators.

---

## Before You Start

1. Read `CLAUDE.md` — all coding standards, security rules, and AI agent rules apply
2. Read `PRODUCT_REQUIREMENTS.md` — understand what is in and out of scope
3. Read `ARCHITECTURE.md` — understand the technical structure
4. Read `SECURITY.md` — understand the security and POPIA requirements

---

## Branch Strategy

| Branch | Purpose |
|---|---|
| `main` | Stable, production-ready code |
| `develop` | Active development integration branch |
| `feature/short-description` | Feature branches |
| `fix/short-description` | Bug fix branches |
| `claude/...` | AI agent development branches |

**Never push directly to `main`.**
All changes go through a branch and pull request.

---

## Commit Message Format

Use imperative mood, present tense, and be specific:

```
Add APS calculator service and unit tests
Fix document upload permission check for SchoolAdmin role
Update README with local setup instructions
```

Bad examples:
```
fixed stuff
changes
wip
```

For significant changes, include a body:
```
Add learner profile POPIA consent field

Required by POPIA to record explicit consent at registration.
Includes consent timestamp and T&Cs version number.
Resolves: #42
```

---

## Pull Request Requirements

Before opening a PR:
- [ ] All tests pass (`dotnet test` / `npm test`)
- [ ] Build succeeds (`dotnet build` / `npm run build`)
- [ ] No secrets committed
- [ ] POPIA and security rules respected
- [ ] Relevant documentation updated
- [ ] Swagger / API docs updated if API changed
- [ ] Migration generated if schema changed (`dotnet ef migrations add ...`)

PR description should include:
- What was changed
- Why it was changed
- How to test it
- Any security or privacy considerations

---

## Code Review Standards

Reviewers should check:
- Does the code follow Clean Architecture layer boundaries?
- Are inputs validated?
- Is RBAC applied correctly?
- Are there tests for the business logic?
- Are there any hardcoded secrets or credentials?
- Is audit logging applied where required?
- Does the change affect POPIA compliance?
- Is the code readable and maintainable?

---

## Testing Standards

- Write tests for all domain logic
- Write tests for all application use cases
- Write integration tests for API endpoints
- Tests must use test data factories, never real learner data
- Tests must not rely on external services (mock them)
- Aim for meaningful coverage on critical paths — avoid 100% coverage theatre

---

## Security and Privacy

- Any change that touches authentication, authorisation, or personal data requires extra review
- POPIA-relevant changes must update `/docs/07-privacy-compliance.md`
- Security-relevant changes must be flagged in the PR description
- Never bypass RBAC checks for convenience

---

## AI Agent Contributions (Claude Code)

AI-generated code follows the same standards as human-written code.
Claude Code must:
- Read `CLAUDE.md` at the start of each session
- Follow all rules in `CLAUDE.md`
- List all files created or modified in the session summary
- Flag any security or privacy concerns explicitly
- Never commit secrets
- Run tests if code exists
