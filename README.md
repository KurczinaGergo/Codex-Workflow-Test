# WorkTrace AI Workflow Workspace

This repository contains the Codex-oriented workflow and living documentation for the WorkTrace MVP.
It keeps project memory, architecture rules, domain rules, progress tracking, and ADR placeholders in one place so planning and implementation sessions can stay aligned.

---

## What is WorkTrace?

WorkTrace is a personal, low-friction developer work-tracking system. It tracks:
- WorkItems as lightweight units of work
- WorkSessions as time intervals
- Notes as contextual knowledge
- Projects as optional grouping

The MVP is single-user, has no authentication, and deliberately avoids workflow enforcement.

---

## Key References

- `AGENTS.md` - Codex session memory and behavior rules
- `docs/architecture.md` - layer model, dependency rules, naming, DI, and placement guidance
- `docs/domain-model.md` - entities, enums, and invariants
- `docs/progress/index.md` - build status and open architectural questions
- `docs/progress/design-state.md` - current architectural ground truth
- `docs/decisions/index.md` - ADR catalog

---

## Repository Structure

```text
/
|-- AGENTS.md
|-- agents/
|   `-- auditor.md
|-- docs/
|   |-- Audit/
|   |-- architecture.md
|   |-- domain-model.md
|   |-- workflow-guide.md
|   |-- specs/
|   |-- decisions/
|   |-- review/
|   `-- progress/
`-- .codex/
    `-- skills/
```

---

## Current Project State

- MVP baseline architecture is documented
- Core aggregates, application handlers, persistence, read models, and API endpoints are implemented in the source project
- Test suites exist for domain, application, infrastructure, and API layers
- The next practical follow-up is running tests and database update commands in an environment with full .NET SDK and NuGet access

---

## Workflow

Use the reasoning partner to plan and write specs, then use Codex to implement and review. The quick operating guide is in `docs/workflow-guide.md`.

Audit mode is available through `agents/auditor.md` and writes workflow-level Mermaid reports plus run statistics into `docs/Audit/<timestamp>/` when the user stops an audit session.
