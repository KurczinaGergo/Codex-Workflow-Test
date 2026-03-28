# Feature Spec - WorkTrace MVP

---

## Context

WorkTrace is intended to be a personal, low-friction developer work-tracking system with a single-user MVP. The repository currently contains workflow and architecture documentation, but the implementation projects are missing from the workspace. This feature rebuilds the documented MVP baseline so the source code matches the documented architecture, domain model, and progress state.

---

## Domain Impact

- New or modified entity: `WorkItem`, `WorkSession`, `Note`, and `Project` aggregates and their strongly typed identifiers
- New domain event: none
- New interface: `IWorkItemRepository`, `IWorkSessionRepository`, `INoteRepository`, `IProjectRepository`, `IUnitOfWork`, `IClock`, `ICurrentUser`, and read-query interfaces for the documented views

---

## Architecture Decisions

- Follow the documented layer ordering exactly: `Domain <- Application <- Infrastructure <- Api`, with `ReadModel` kept separate from write-side repositories.
- Keep core business flow explicit in command handlers; do not use domain events to drive MVP behavior.
- Use EF Core with SQLite for persistence and apply migrations automatically on API startup.
- Model the MVP as single-user by supplying a fixed configured `ICurrentUser` from the API host.
- Return application command results with structured errors and optional warnings so the API can surface non-blocking behavior.
- Keep the API thin with Minimal API endpoint mapping and serve a static UI from `wwwroot`.

---

## Implementation Scope - What the agent should do

- [ ] Create the .NET 8 solution structure with `Api`, `Application`, `Domain`, `Infrastructure`, `ReadModel`, and matching test projects.
- [ ] Implement the documented domain model, invariants, value objects, and domain exceptions for work items, sessions, notes, and projects.
- [ ] Implement application abstractions, command/query DTOs, validators, handlers, result types, and warning/error code definitions.
- [ ] Implement EF Core SQLite persistence, entity mappings, repositories, unit of work, clock/current-user adapters, and startup migration support.
- [ ] Implement read-model query services for active work, work item list, work item detail, timeline, and project list.
- [ ] Implement Minimal API endpoints, Swagger, static file hosting, and a lightweight web UI for the MVP flows.
- [ ] Add domain, application, infrastructure, and API tests covering happy paths and critical edge cases.
- [ ] Generate review workflow artifacts after implementation using the repository's review-ready process.

## Out of Scope - What the agent must NOT do

- Authentication, authorization, or multi-user behavior beyond the fixed configured MVP user
- Analytics, reporting, background jobs, or event-driven orchestration outside the documented MVP

---

## Test Expectations

- Unit tests required for: aggregate invariants, command validation/handler behavior, repository/query behavior, and API endpoint flows
- Edge cases to cover: blank work item title auto-generation, done-state timestamp behavior, positive work-session duration, duplicate active-session prevention, note text validation, archive/restore orchestration, and restrictive delete behavior

---

## Open Questions

- Should the static UI be a single-page dashboard or a small multi-section page for the MVP?
- Should the infrastructure project commit an initial migration snapshot now, or should the database be created from the model at runtime for this workspace bootstrap?
