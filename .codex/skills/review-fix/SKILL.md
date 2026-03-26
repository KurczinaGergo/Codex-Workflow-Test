---
name: review-fix
description: Apply review feedback from a reasoning partner session surgically. Invoke when you have specific feedback to implement from a Desktop or planning session.
---

Applies review feedback from a reasoning partner session surgically.
Never read `_`-prefixed files.

## Before touching anything
Read the code and feedback in full. Categorize each issue:
- **Architecture** → flag explicitly, propose fix aligned with architecture rules. If resolution requires a human decision, surface it as an open question — do not resolve unilaterally.
- **Logic** → fix it, explain why, add or update a unit test.
- **Style** → apply silently.

## Rules
- Surgical fixes only — do not refactor unrelated code in the same pass
- If a fix reveals a deeper problem, note it — do not fix it silently
- If feedback contradicts an existing ADR, flag the conflict — do not resolve unilaterally
- Run affected tests after every fix — do not report completion with broken tests
- Do not update `docs/progress/` unless the fix changes something already recorded there
- Do not run git commands

## Report
What changed and why | which test covers the fixed case | any deeper problem flagged for reasoning partner
