# Feature Progress - worktrace-mvp

**Branch**: `not captured`
**Spec**: `docs/specs/worktrace-mvp.md`
**Review file**: `docs/review/worktrace-mvp-review.md`
**Status**: In Progress
**Merged to**: `not merged`

---

## What was built

The WorkTrace MVP was bootstrapped into a runnable .NET 8 solution with Domain, Application, Infrastructure, ReadModel, and Api projects plus matching tests. The implementation now includes the documented aggregates, command handlers, SQLite persistence, read-model queries, a static web UI, Minimal API endpoints, and Swagger. The full automated test suite passes in the current environment. The remaining work is architectural cleanup so the API composes the lower layers instead of using an in-memory bootstrap store.

---

## Changed files

| File | Layer | Change |
|------|-------|--------|
| `src/WorkTrace.Domain/**` | Domain | Added aggregates, identifiers, guards, enums, and domain errors |
| `src/WorkTrace.Application/**` | Application | Added abstractions, commands, handlers, validators, query contracts, and result types |
| `src/WorkTrace.Infrastructure/**` | Infrastructure | Added EF Core persistence, repositories, adapters, unit of work, and DI |
| `src/WorkTrace.ReadModel/**` | ReadModel | Added DTOs, query interfaces, query implementations, and DI |
| `src/WorkTrace.Api/**` | Api | Added host startup, endpoints, static UI, Swagger, and bootstrap store |
| `tests/WorkTrace.Domain.Tests/**` | Tests | Added domain behavior tests |
| `tests/WorkTrace.Application.Tests/**` | Tests | Added application handler tests |
| `tests/WorkTrace.Infrastructure.Tests/**` | Tests | Added persistence and read-model tests |
| `tests/WorkTrace.Api.Tests/**` | Tests | Added API and static-host tests |
| `docs/specs/worktrace-mvp.md` | Docs | Added feature spec |
| `docs/review/worktrace-mvp-review.md` | Docs | Added review bridge |
| `docs/progress/index.md` | Docs | Updated baseline and deviations |

---

## Decisions made during implementation

- Split implementation into three tracks - better parallelism and clearer ownership - rejected a fully serial build.
- Used SQLite-backed infrastructure tests - validates restrictive foreign keys and active-session uniqueness - rejected in-memory-only persistence tests.
- Kept the API functional with an in-memory bootstrap store - allowed the UI/API track to finish independently - rejected blocking the API until lower-layer integration was complete.

---

## Deviations from spec

- The API is not yet a thin composition layer over Application, Infrastructure, and ReadModel; it currently uses `MvpWorkTraceStore` inside the API project.
- Infrastructure duplicates `IClock`, `ICurrentUser`, and `IUnitOfWork` even though the intended architecture places them in Application.

---

## Proposed documentation updates

- `docs/architecture.md` -> record the temporary bootstrap store and duplicate runtime abstractions, or remove them in the next integration pass.
- `docs/progress/design-state.md` -> clarify that the MVP is functionally implemented but not yet fully aligned with the intended API composition design.
- New ADR required: yes

---

## Open questions raised

- Should the bootstrap API store be replaced immediately in the next pass, or treated as an accepted short-lived transition?
- Should the duplicate Infrastructure abstractions be bridged to Application first, or removed in a single cleanup pass?
