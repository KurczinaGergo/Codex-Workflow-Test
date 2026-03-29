# Workflow Guide

This repository uses a two-role AI workflow for WorkTrace: a reasoning partner handles planning and design, while Codex handles implementation and repository operations.

An optional audit overlay can be enabled by the user. When active, `AuditorAgent` records workflow interactions and emits audit artifacts when the user ends the audit.

---

## The Two Roles

| Role | What it does | WorkTrace examples |
|------|--------------|--------------------|
| **Reasoning partner** | Planning, trade-offs, spec writing, doc updates | Refine aggregate rules, decide architecture changes, write feature specs |
| **Implementation agent** | Edit files, run tests, verify flows, prepare review output | Implement handlers, wire endpoints, update progress docs |

---

## Core Loop

```text
Reasoning partner -> spec -> Implementation agent
                              |
                         /review-ready
                              |
              +---------------+----------------+
              |                                |
              v                                v
   docs/review/*-review.md         docs/progress/ patched
              |
              v
   Reasoning partner reviews changes, decides doc updates,
   records ADRs if needed, then the human commits and merges
```

---

## Audit State

Audit state is explicit and user-controlled.

- The audit starts only when the user requests it
- The audit stops only when the user requests it
- While active, `AuditorAgent` records workflow events about users, agents, skills, and agent-to-agent communication
- The audit covers the workflow itself, not the implementation project's internal source-code behavior
- Every agent instance is logged as a distinct entity, even when multiple instances use the same agent markdown file

The user is also part of the audit trail. User prompts should appear in the audit data with a short one-line summary that can be reused in Mermaid edge labels.

---

## Audit Outputs

When the user stops an audit session, the workflow should create:

`docs/Audit/<audit_timestamp>/`

Expected contents:

| File | Purpose |
|------|---------|
| `sequence.mmd` | Time-ordered Mermaid sequence diagram of user and agent interactions |
| `topology.mmd` | Mermaid connection graph showing agent-instance relationships and aggregated call counts |
| `stats.md` | Human-readable session and per-agent run statistics |
| `events.json` | Raw workflow event log for regeneration or later analysis |
| `index.html` | Browser-openable report that renders the diagrams and statistics |

The sequence diagram should show why one entity contacted another using a one-line summary on each edge. The topology graph should label each edge with the number of calls between the same two entities.

---

## File Responsibilities

| File | Who writes it | Who reads it | Rule |
|------|---------------|--------------|------|
| `docs/specs/*.md` | Human via reasoning partner | Implementation agent | One file per feature |
| `docs/decisions/index.md` | Human | Implementation agent | Read before any ADR file |
| `docs/decisions/ADR-*.md` | Human via reasoning partner | Implementation agent | Load selectively by tags |
| `docs/review/*-review.md` | Implementation agent | Human + reasoning partner | Keep concise and actionable |
| `docs/progress/index.md` | Implementation agent | Human + reasoning partner | Incrementally update project status |
| `docs/progress/design-state.md` | Human after review | Reasoning partner + implementation agent | Current architectural ground truth |
| `docs/architecture.md` | Human-maintained living doc | Implementation agent | Read before cross-layer changes |
| `docs/domain-model.md` | Human-maintained living doc | Implementation agent | Read before domain changes |
| `AGENTS.md` | Human | Implementation agent | Behavior rules and navigation only |

---

## Context Rules

- `AGENTS.md` contains behavior and navigation, not deep technical detail
- `_`-prefixed files are templates and should not be loaded during normal implementation work
- Load docs on demand instead of reading the entire documentation tree
- Read `docs/decisions/index.md` before opening ADR files
- Treat `docs/progress/design-state.md` as the current source of truth if architecture has drifted

---

## WorkTrace-Specific Planning Checklist

- Read `docs/progress/index.md` before planning new work
- Check `docs/progress/design-state.md` for active architectural constraints
- Review `docs/architecture.md` for layer boundaries and DI ownership
- Review `docs/domain-model.md` for WorkItem, WorkSession, Note, and Project rules
- Check whether the work touches out-of-scope MVP areas such as auth, multi-user support, analytics, or event-driven core logic

---

## Current Baseline

- MVP architecture and domain docs are populated
- Core implementation phases are documented as completed
- No ADRs are recorded yet
- The main follow-up task is validating the solution in an environment with full .NET SDK and package access
