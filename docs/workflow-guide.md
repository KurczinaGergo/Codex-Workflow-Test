# Workflow Guide

This repository uses a two-role AI workflow. Quick reference — full reasoning in the [blog post](https://your-handle.github.io/your-blog-post).

---

## The two roles

| Role | What it does | Examples |
|------|-------------|---------|
| **Reasoning partner** | Planning, architecture, design trade-offs, spec writing | Claude Desktop, ChatGPT, Gemini Advanced |
| **Implementation agent** | Writing code, running tests, operating across the repo | Codex CLI, Copilot Workspace, Cursor |

---

## Core loop

```
Reasoning partner → spec → Implementation agent
                               │
                          /review-ready
                               │
              ┌────────────────┴────────────────┐
              ▼                                 ▼
   docs/review/*-review.md          docs/progress/ patched
              │
              ▼
   Reasoning partner (next session)
     reads progress + review → decides on doc updates → writes ADRs
              │
              ▼
   Human commits → PR → merge
```

---

## File responsibilities

| File | Who writes it | Who reads it | Rule |
|------|--------------|--------------|------|
| `docs/specs/*.md` | Human via reasoning partner | Implementation agent | One file per feature |
| `docs/decisions/index.md` | Human | Implementation agent | ADR tag catalog — read before loading ADRs |
| `docs/decisions/ADR-*.md` | Human via reasoning partner | Implementation agent | Load selectively via index tags |
| `docs/review/*-review.md` | Implementation agent | Human + reasoning partner | Max 150 lines |
| `docs/progress/index.md` | Implementation agent | Human + reasoning partner | Incremental patch on each `$review-ready` |
| `docs/progress/design-state.md` | Human after review | Reasoning partner | ADR index, active constraints |
| `docs/progress/features/*.md` | Implementation agent | Human | One per branch, never modified after merge |
| `docs/progress/archive/*.md` | Implementation agent | Human | Overflow from index, append-only |
| `docs/architecture.md` | Human after review | Implementation agent | Max 150 lines — split if larger |
| `docs/domain-model.md` | Human after review | Implementation agent | Max 150 lines — split if larger |
| `AGENTS.md` | Human | Implementation agent | Behavior rules only — no technical content |

---

## Context window rules

These rules prevent unnecessary token consumption as the project grows:

- **`AGENTS.md` contains behavior, not knowledge.** Technical content lives in `docs/` only. No duplication.
- **`_`-prefixed files are never read by the agent.** They are human-facing templates.
- **Docs load on demand.** Each skill specifies which files to load. Nothing loads upfront.
- **ADRs load selectively.** Read `docs/decisions/index.md`, load only ADRs whose tags match the feature's layers.
- **`docs/progress/features/` never loads wholesale.** Access only the specific file needed.
- **`docs/progress/index.md` patches incrementally.** `$review-ready` patches one row — never regenerates from all feature files.
- **Size limits.** Review files: 150 lines. `docs/architecture.md` and `docs/domain-model.md`: 150 lines. Split into sub-files if exceeded.

---

## Skill reference

| Command | When to use |
|---------|-------------|
| `$new-feature` | Start implementing a specced feature |
| `$review-ready` | Feature implementation is complete |
| `$review-fix` | Apply feedback from a reasoning partner session |

---

## Checklist: before a planning session

- [ ] Read `docs/progress/index.md` — what has been built?
- [ ] Read `docs/progress/design-state.md` — which ADRs are active?
- [ ] Read the relevant review file if a feature was recently merged
- [ ] Check Open Architectural Questions in `docs/progress/index.md`

## Checklist: before merging a feature branch

- [ ] Review file exists in `docs/review/` and is under 150 lines
- [ ] Feature file exists in `docs/progress/features/`
- [ ] `docs/progress/index.md` is patched and up to date
- [ ] Proposed doc updates from the review file have been decided (accepted or explicitly deferred)
- [ ] Any new ADRs are written, tagged, and added to `docs/decisions/index.md`
