# 20 — Risk Register

## Title
FundiLink — Risk Register

## Purpose
Identify, assess, and plan mitigations for key risks facing FundiLink across product, business, legal, and technical dimensions.

---

## Risk Rating Scale

| Likelihood | Impact | Rating |
|---|---|---|
| Low + Low | = | Low |
| Low + High or High + Low | = | Medium |
| High + High | = | High / Critical |

---

## Product Risks

| # | Risk | Likelihood | Impact | Rating | Mitigation |
|---|---|---|---|---|---|
| P1 | APS calculator produces incorrect scores | Medium | High | High | Comprehensive unit tests; validated against official NSC APS tables; flag Life Orientation edge cases |
| P2 | Programme data is out of date or inaccurate | High | Medium | High | Source from official public data; add "last verified" date; disclaimer in UI; manual review process |
| P3 | Learners confuse FundiLink for an official admissions portal | Medium | High | High | Clear disclaimers on all programme and application screens; in T&Cs; repeat in onboarding |
| P4 | Low profile completion rates (learners drop off) | Medium | High | High | Progressive disclosure; celebrate small milestones; send completion nudge notifications |
| P5 | Document upload failures on mobile / low bandwidth | High | Medium | High | Optimise upload UX; retry logic; show clear progress; allow background upload |

---

## Business Risks

| # | Risk | Likelihood | Impact | Rating | Mitigation |
|---|---|---|---|---|---|
| B1 | Learner acquisition is slow without school channel | Medium | High | High | Prioritise school pilot; WhatsApp sharing; NGO partnerships |
| B2 | Schools don't adopt the dashboard | Medium | High | High | Start with motivated counsellors; reduce friction to value |
| B3 | Revenue model doesn't work at scale | Medium | High | High | Validate pricing with market research before building payment features |
| B4 | A well-funded competitor enters the market | Low | High | Medium | Build network effects early; focus on mobile-first and rural differentiator |
| B5 | Grant or seed funding not secured | Medium | High | High | Apply to multiple funders; demonstrate pilot traction before asking for large rounds |

---

## Legal and Compliance Risks

| # | Risk | Likelihood | Impact | Rating | Mitigation |
|---|---|---|---|---|---|
| L1 | POPIA breach due to inadequate data protection | Low | Critical | High | Privacy-by-design; role-based access; audit logging; POPIA compliance checklist; legal review |
| L2 | Processing minors' data without guardian consent | Low | Critical | High | Guardian consent flow enforced for under-18; not optional |
| L3 | FundiLink misrepresents itself as an official portal | Low | High | Medium | Clear disclaimers everywhere; T&Cs; training for support agents |
| L4 | Liability for incorrect programme eligibility guidance | Medium | Medium | Medium | Disclaimer in UI; "results are indicative, verify with institution"; no guarantee language |
| L5 | Document storage breach (ID documents, personal data) | Low | Critical | High | Encrypted storage; access control; no public URLs; security audit before launch |

---

## Technical Risks

| # | Risk | Likelihood | Impact | Rating | Mitigation |
|---|---|---|---|---|---|
| T1 | Secrets committed to repository | Medium | Critical | High | `.gitignore` covers secrets; CLAUDE.md rules; pre-commit hook consideration; secrets scanning |
| T2 | Database migration failure in production | Low | High | Medium | Always test migrations on staging; keep migration scripts; backup before migrating |
| T3 | JWT token security misconfiguration | Low | High | Medium | Use proven ASP.NET Identity defaults; security review of auth implementation |
| T4 | Unauthorised access to learner documents | Low | Critical | High | Access control on every document endpoint; audit logged; no public URLs; tested |
| T5 | Application instability at scale | Low | Medium | Low | Load testing before launch; horizontal scaling plan; health check monitoring |
| T6 | AI agent (Claude Code) introduces a security vulnerability | Medium | High | High | Review all AI-generated security code; CLAUDE.md agent rules; security-focused PR review |

---

## Risk Review Schedule

- Review risk register at the start of each phase
- Update ratings after incidents or near-misses
- Add new risks as the product and context evolve
- Assign an owner to each High/Critical risk

---

## Next Actions
- Assign risk owners to all High/Critical risks
- Define incident response plan for L1 (POPIA breach)
- Define incident response plan for T4 (document access breach)
- Add pre-commit hooks for secrets detection (consider `git-secrets` or `detect-secrets`)
- Schedule security review before public launch
