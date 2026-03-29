# Project Memory - Codex CLI

---

## File Loading Rules

Never read files prefixed with `_` - these are human-facing templates, not instructions.
Never load `docs/progress/features/` wholesale - read only specific files when a task requires it.
Lazy loading: only read a docs/ file when the current task explicitly requires it.
The files below are read on demand:
- `docs/architecture.md` - read when implementing or checking layer and dependency rules
- `docs/domain-model.md` - read when touching WorkItem, WorkSession, Note, or Project behavior
- `docs/decisions/index.md` - read before loading any ADRs
- `docs/progress/index.md` - read at the start of feature planning or review work
- `docs/progress/design-state.md` - read when architecture or cross-cutting constraints may have drifted

---

## Project Overview

WorkTrace is a personal developer work-tracking system for capturing lightweight work items, time sessions, notes, and optional project grouping. The MVP is intentionally single-user and low-friction: it helps developers record work quickly without turning the product into a workflow engine. The core domain centers on explicit work state, tracked effort, and contextual notes.

---

## Tech Stack

- **Backend**: C# / .NET 8, ASP.NET Core Minimal APIs
- **Frontend**: Static web UI served from the API project (`wwwroot`)
- **Database**: EF Core with SQLite
- **Testing**: xUnit, NSubstitute, integration tests for infrastructure and API

---

## Solution Structure

```text
WorkTrace.sln

agents/
  architect.md
  auditor.md
  code_review.md
  documenting.md
  sw_technical_engineer.md

docs/
  Audit/
  architecture.md
  roadmap.md
  decisions/
  dod/
  tasks/

src/
  WorkTrace.Api/
  WorkTrace.Application/
  WorkTrace.Domain/
  WorkTrace.Infrastructure/
  WorkTrace.ReadModel/

tests/
  WorkTrace.Api.Tests/
  WorkTrace.Application.Tests/
  WorkTrace.Domain.Tests/
  WorkTrace.Infrastructure.Tests/
```

---

## Behavioral Rules - Non-Negotiable

- Never read `_`-prefixed files unless the human explicitly asks for a template review
- Never add new agents, skills, or workflow files unless the human explicitly requests them
- `agents/auditor.md` is workflow-only: it audits agent coordination, not the target project
- Never run git commands unless the human asks for version-control help
- Never make independent architecture changes when docs and code disagree - surface the conflict and ask
- Read `docs/architecture.md` before changing cross-layer behavior or dependency structure
- Read `docs/domain-model.md` before changing aggregate behavior or invariants
- Treat `docs/progress/design-state.md` as the current architectural ground truth when it differs from older docs
- Never commit secrets, connection strings with credentials, or machine-specific overrides

Technical architecture rules live in `docs/architecture.md`.

---

## Skills

Invoke skills by typing `$skill-name` in the Codex composer, or run `/skills` to browse.

| Skill | When to use |
|-------|-------------|
| `$new-feature` | Start implementing a specced feature |
| `$review-ready` | Feature implementation is complete |
| `$review-fix` | Apply feedback from a reasoning partner session |

Skill definitions are in `.codex/skills/`.
