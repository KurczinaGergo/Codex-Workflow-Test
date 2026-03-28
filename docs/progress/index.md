# Build Progress - Feature Index

**Last updated**: 2026-03-28, MVP bootstrapped and tested

---

## How to Read This File

- **Completed**: implemented and reflected in the current baseline
- **In Progress**: active work not yet considered part of baseline
- **Planned**: next concrete follow-up work
- **Backlog**: intentionally out-of-scope or deferred ideas

Check **Deviations** before planning features that touch the affected area.

---

## Completed

- Solution scaffolding is in place with `Api`, `Application`, `Domain`, `Infrastructure`, `ReadModel`, and matching test projects
- Domain aggregates are implemented for `WorkItem`, `WorkSession`, `Note`, and `Project`
- Application commands, handlers, validation, warnings, and common result types are implemented
- EF Core SQLite persistence, repositories, migrations, unit of work, and guard-rail constraints are implemented
- Read model queries are implemented for active work, work item list, work item detail, timeline, and project list
- Minimal API endpoints, Swagger, and the static UI host are implemented
- Domain, application, infrastructure, and API test suites are present in the solution
- `dotnet test .\WorkTrace.sln` passes in the current environment
- Review bridge and feature progress records are captured in the source project documentation

---

## In Progress

_Nothing in progress._

---

## Planned

- Wire the API host to the Application, Infrastructure, and ReadModel layers so the API becomes the thin composition root described in `docs/architecture.md`
- Consolidate runtime abstractions so `IClock`, `ICurrentUser`, and `IUnitOfWork` live in the intended layer only
- Run database initialization/update flow in an environment with full EF tooling access

---

## Backlog

- Authentication and multi-user support if scope expands beyond the personal MVP
- Analytics and reporting beyond the current operational views
- Event-driven core business logic
- Richer sharing or team-level aggregation workflows

---

## Deviations from Original Design

| Area | Original design | Actual implementation | Reason | ADR |
|------|-----------------|-----------------------|--------|-----|
| API composition | API stays thin and composes lower layers | API currently uses `MvpWorkTraceStore` inside `src/WorkTrace.Api` | The API/UI track was completed before lower-layer integration was reconciled | _(none yet)_ |
| Bootstrap contracts | Application owns `IClock`, `ICurrentUser`, and `IUnitOfWork` abstractions | Infrastructure hosts duplicate bootstrap contracts | The infrastructure track shipped independently before architecture convergence | _(none yet)_ |

---

## Open Architectural Questions

- When the MVP moves beyond local development, should SQLite remain the default or should PostgreSQL become the primary runtime target?
- If multi-user support is introduced, should `ICurrentUser` remain host-configured or move behind real authentication/identity plumbing?
- Should the bootstrap API store be removed immediately in the next pass, or accepted temporarily while the layered integration is completed?
- Should the duplicate `IClock`, `ICurrentUser`, and `IUnitOfWork` contracts be bridged first or removed in one cleanup step?
