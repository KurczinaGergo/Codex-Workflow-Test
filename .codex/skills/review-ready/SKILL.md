---
name: review-ready
description: Generate the review bridge file and update the progress tracker. Invoke when feature implementation is complete and ready for reasoning partner review.
---

Generates the review bridge file and updates the progress tracker.
Never read `_`-prefixed files. Never load `docs/progress/features/` wholesale.

## Step 1 — Create `docs/review/<feature-name>-review.md`

File must be self-contained and fit in one reasoning partner context window.
**Hard limit: 150 lines.** If you exceed this, cut section 4 (excerpts) first, then shorten section 3.
Prefer short and precise over comprehensive.

Required sections:

**1. Summary** — 3–5 sentences: what was built, which spec, current project phase.

**2. Changed files** — table: file | layer | new/modified/deleted. Structure only, no code.

**3. Decisions** — every non-trivial independent decision.
Format per decision: What / Why / Alternative rejected.
If none: _None — followed spec exactly._

**4. Critical excerpts** — max 3–5 snippets.
Include only: new entity/interface definitions, non-trivial algorithms, spec deviations.
Exclude: boilerplate, config, DTOs, DI registration.

**5. Spec deviations** — every difference from the spec with explanation. If none: _None._

**6. Open questions** — decisions requiring human judgment before the next feature. If none: _None._

**7. Test coverage** — tests added (name + what it asserts) | gaps and reason.

**8. Proposed doc updates** — specific file + section for each change needed.
Format: `docs/architecture.md` → [what to change]
New ADR required: yes/no — if yes, include a one-paragraph draft.

## Step 2 — Create `docs/progress/features/<feature-name>.md`

Use `docs/progress/features/_feature-template.md` as structure reference only — do not copy comments.
Fill in: branch, spec path, changed files, decisions, deviations, proposed doc updates, open questions.

## Step 3 — Patch `docs/progress/index.md`

Read `docs/progress/index.md` only. Do NOT read other files in `docs/progress/features/`.
Apply targeted changes:
- Move this feature from In Progress → Completed (one row)
- Append new deviations to the Deviations table
- Append open questions to the Open Architectural Questions section
- Update the Last updated date

If Completed exceeds 10 entries: move oldest 5 rows to
`docs/progress/archive/<year>-<quarter>.md` (create if it does not exist).
Do not modify `docs/progress/design-state.md`.

## On completion
Report all created and modified files. Do not run git commands.
