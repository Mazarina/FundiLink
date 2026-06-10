# Phase 7 — AI Guidance Assistant (MVP)

You are building Phase 7 of FundiLink by ZulTek on branch `claude/happy-dirac-n7qgtg`.

FundiLink is a South African student opportunity platform (Clean Architecture: ASP.NET Core 8 backend, React+Vite+TypeScript frontend). Phases 1–6 are complete and green (all .NET + frontend tests pass, both build clean).

**FIRST**: Read `/home/user/FundiLink/CLAUDE.md` fully. Key rules:
- NEVER commit secrets, API keys, or credentials. The AI provider key MUST come from environment variables / user-secrets only — placeholder `SET_VIA_ENV` in `appsettings.json`.
- dotnet is at `/usr/local/dotnet/dotnet`.
- Push to `claude/happy-dirac-n7qgtg` only.
- All existing tests must keep passing.
- Follow existing CQRS/MediatR patterns (`Features/Applications/`, `Features/ProgrammeMatching/`, `Features/Bursaries/`, `Features/Admin/`) and the Phase 5 stub-behind-interface pattern (WhatsApp/SMS) and Phase 6 disclaimer pattern.

**CRITICAL POSITIONING / SAFETY:**
- FundiLink is NOT an official advisor. AI output is **guidance only** and must never fabricate institution, programme, bursary, or NSFAS facts (no hallucinated APS cut-offs, deadlines, amounts, or contact details).
- NO real LLM provider call in this phase by default — the assistant must sit behind an `IAiAssistantService` interface with a **deterministic stub implementation** (rule/template-based using the learner's own profile + APS + their matched programmes/bursaries). A real provider may be wired later behind the same interface, key via env only.
- Every AI response carries a guidance-only disclaimer and a "verify with the official institution/funder / talk to a support agent" note.

## Scope — MVP only

A learner-facing guidance assistant that answers a constrained set of profile-aware questions (what do I qualify for, what is my APS, what documents do I still need, which bursaries may fit me), grounded ONLY in the learner's existing FundiLink data. No free-form open-domain chat with an external model in this phase.

### 1. Domain / Application
- `IAiAssistantService` interface in `Application/Common/Interfaces/` with a typed request/response (no `dynamic`/`object`).
- `Features/Assistant/` CQRS: `AskAssistant` command/query — takes the learner's question (from a small enum or validated intent set) + UserId, loads the learner + APS + matches via existing repositories, returns a grounded `AssistantResponseDto` with `answer`, `sources` (which FundiLink data was used), `guidanceOnly: true`, and a disclaimer string.
- Deterministic stub `RuleBasedAiAssistantService` in Infrastructure implementing the interface — no external calls. Register in `DependencyInjection.cs`.
- Append-only audit/log of assistant interactions if it touches sensitive data (reuse notification/audit log patterns; POPIA-minimal — do not store more than necessary).

### 2. API
- `AssistantController` (`api/v1/assistant`) `[Authorize]`, owner-scoped (uses the caller's identity; never another learner's data).
- Request DTO in `Models/Requests.cs`. Validate the intent at the API boundary.

### 3. Frontend
- `src/features/assistant/assistantApi.ts`, an `AssistantPage` (constrained question chips/intents, grounded answers, clear guidance-only disclaimer, link to support).
- Route in `App.tsx` (ProtectedRoute) + an "Ask FundiLink" tile on `ProfilePage`.
- Follow existing Tailwind/brand + error/loading patterns.

### 4. Tests
- Backend: stub assistant returns grounded answers from profile/APS; owner-scope enforced (unauthorised rejected); no-profile path handled; intent validation (>= 4 tests).
- Frontend: >= 3 tests (renders disclaimer, renders a grounded answer from mocked API, intent selection triggers API call).

### 5. Execution steps
1. Read CLAUDE.md, then Phase 3/6 features as templates.
2. Domain/Application -> Infrastructure (stub + DI) -> API -> Frontend -> Tests.
3. `/usr/local/dotnet/dotnet build FundiLink.sln` and `dotnet test` — all green.
4. Frontend `npm run build` and `npm test -- --run` — clean.
5. Update `ROADMAP.md` Phase 7 checklist (mark MVP delivered; real LLM provider / open-domain chat / human-handoff automation deferred).
6. Write the Phase 8 prompt into `prompts/NEXT_PROMPT.md`.
7. Commit: `Add Phase 7 AI guidance assistant (stub)`.
8. Push: `git push -u origin claude/happy-dirac-n7qgtg`.

**Security/privacy notes for commit message:**
- No real LLM provider integration — deterministic stub behind `IAiAssistantService`; any future key via env only.
- AI output is guidance only, grounded strictly in the learner's own FundiLink data; no fabricated institution/funder facts.
- Assistant is owner-scoped; sensitive access minimised per POPIA.
- No secrets committed.
