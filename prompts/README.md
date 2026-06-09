# Prompts — FundiLink

## Purpose
This folder logs all Claude Code prompts used to build FundiLink. It serves as a history of what was built and why, and provides the next recommended prompt for each development phase.

---

## How Prompts Are Structured

Each prompt file follows this naming convention:
```
NNN-short-description.md
```

Where `NNN` is a zero-padded sequence number (000, 001, 002, ...).

Each prompt file contains:
- **Prompt number and title**
- **Phase** it belongs to
- **Purpose** — what this prompt is trying to achieve
- **Context** — what existed before this prompt was run
- **The prompt** — the full text of the prompt as it was given to Claude Code
- **Result** — what was created or changed (filled in after the prompt runs)

---

## Current Prompt Log

| # | File | Phase | Description | Status |
|---|---|---|---|---|
| 000 | `000-initial-project-setup.md` | Phase 0 | Create documentation foundation | Complete |

---

## Writing Future Prompts

When writing a new prompt for Claude Code, follow these guidelines:

### A Good Prompt Includes:
1. **Context** — What already exists? What phase is this?
2. **Goal** — What should be built or created?
3. **Specific files and folders** — List exactly what should be created or modified
4. **Tech requirements** — Framework, patterns, naming conventions
5. **Security and POPIA requirements** — Always include
6. **Exclusions** — What should NOT be done in this prompt
7. **Output requirements** — What should Claude report back after completion

### Rules for All Future Prompts:
- Always start: "Read `CLAUDE.md` before starting."
- Always specify: no secrets committed
- Always specify: run build/tests if code is created
- Always specify: update documentation if architecture changes
- Always specify: list files created/modified at the end

---

## NEXT_PROMPT.md

The file `NEXT_PROMPT.md` always contains the recommended next prompt to run.
Update it after completing each phase.
