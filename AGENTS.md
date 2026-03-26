# Project Memory — Codex CLI

<!-- FILL IN: This file is loaded by Codex CLI automatically at the start of every session.
     Keep it accurate and concise. Long files slow down context loading.
     When something changes in the project, update this file in the same commit.
     Remove all comment blocks before committing real content.
     IMPORTANT: This file contains ONLY behavioral rules and navigation pointers.
     Technical content (entities, layer rules, naming conventions) lives in docs/ only.
     Never duplicate technical content here — if it belongs in docs/, put it there. -->

---

## File Loading Rules

**Never read files prefixed with `_`** — these are human-facing templates, not instructions.
**Never load `docs/progress/features/` wholesale** — read only specific files when a task requires it.
**Lazy loading:** only read a docs/ file when the current task explicitly requires it.
The files below are NOT auto-loaded — they are read on demand only:
- `docs/architecture.md` — read when implementing or checking architecture rules
- `docs/domain-model.md` — read when touching domain entities
- `docs/decisions/index.md` — read when selecting relevant ADRs for a feature
- `docs/progress/index.md` — read at the start of every new-feature task

---

## Project Overview

<!-- FILL IN: 2–4 sentences. What does this project do? Who uses it? What is the core domain? -->

_Describe your project here._

---

## Tech Stack

<!-- FILL IN: Key technologies and versions. -->

- **Backend**:
- **Frontend**:
- **Database**:
- **Testing**:

---

## Solution Structure

```
<!-- FILL IN: paste your solution/project folder tree here -->
```

---

## Behavioral Rules — Non-Negotiable

<!-- FILL IN: How Codex must behave in this project.
     Rules only — no technical architecture content.
     Examples below — replace with your own. -->

- Never run git commands — the human handles all version control
- Never make architecture decisions independently — flag conflicts, propose options, wait for confirmation
- Never read `_`-prefixed files
- Never put business logic in [your API/controller layer]
- Never reference [your infrastructure layer] from [your domain layer]
- Never commit secrets or API keys

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
