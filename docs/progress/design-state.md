# Build Progress - Architectural Ground Truth

**Last updated**: 2026-03-28

---

## ADR Index

| ADR | Title | Status | Affects |
|-----|-------|--------|---------|
| _(none yet)_ | | | |

---

## Active Design Constraints

- The Domain layer owns local business invariants and should not depend on infrastructure concerns
- The Application layer owns orchestration, warnings, validation flow, and transaction boundaries
- One command equals one transaction
- Read models stay separate from write-side repositories
- Core business flow is explicit; domain events are not used to drive MVP core logic
- The API layer stays thin and acts as the composition root
- The runtime remains single-user in the MVP through a fixed configured `ICurrentUser`
- SQLite is the current persistence target and migrations are part of startup flow
- No cascade deletes; restrictive foreign keys and uniqueness constraints enforce safety

---

## Design Evolution Log

- 2026-03-28: Baseline architecture and domain state captured in this workflow repository from the current WorkTrace MVP documentation set

---

## Known Divergences from docs/architecture.md

| Area | Current reality | Planned doc update |
|------|-----------------|--------------------|
| _(none)_ | | |
