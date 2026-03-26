---
name: new-feature
description: Implement a specced feature across all architectural layers. Invoke when a spec exists in docs/specs/ and you are ready to start implementation.
---

## Load before starting
1. `docs/progress/index.md` — current build state and deviations
2. `docs/specs/<feature-name>.md` — the spec (must exist before proceeding)
3. `docs/architecture.md` — layer rules and naming conventions
4. `docs/decisions/index.md` — read tags, then load only ADRs relevant to this feature's layers

Never read `_`-prefixed files. Never load `docs/progress/features/` wholesale.

## If no spec exists
Feature touches multiple layers → ask: "Create a spec draft first, or proceed?"
Single-layer fix / rename / config tweak → proceed directly.

## Implementation order
1. Domain: entities, value objects, events, repository interfaces
2. Application: commands/queries, handlers, validators
3. Infrastructure: persistence config, repository implementations, adapters
4. API: thin controllers/endpoints, real-time events if needed

Verify every class satisfies the project's architecture rules before marking it done.

## Tests
Cover happy path and critical edge cases.
No previously-passing test may be broken on completion.

## On completion
Run `$review-ready`. Do not report completion without it.

## Report
Files created | files modified | tests added | manual steps (migrations, env vars, config)
Do not run git commands.
