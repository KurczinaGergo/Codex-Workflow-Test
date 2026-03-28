# Architecture Reference

---

## Layer Overview

```text
Domain <- Application <- Infrastructure <- Api
                         ^
                         |
                      ReadModel
```

Rules:
- Domain contains aggregates, IDs, enums, guards, and domain exceptions only
- Application depends only on Domain and owns command orchestration, validation, warnings, and transaction boundaries
- Infrastructure depends on Domain and Application and implements repositories, EF Core persistence, unit of work, and clock access
- ReadModel uses the persistence layer for query access but stays separate from write-side repositories and command handlers
- Api is a thin composition layer: DI, endpoint mapping, static file hosting, Swagger, and startup migration only
- One command equals one transaction
- Cross-aggregate rules live in handlers, not inside aggregates

---

## Key Design Decisions

### Hybrid DDD with explicit orchestration
Aggregates enforce local invariants while handlers coordinate multi-aggregate behavior. This keeps business flow explicit and easy to trace.

### CQRS-lite read separation
Read models are separate from write-side repositories, but the MVP uses the same SQLite database and EF Core context. The goal is query separation, not distributed complexity.

### Warnings are non-blocking
Handlers return `CommandResult<T>` with optional warnings so the system can preserve user freedom without enforcing workflow rules.

### Database constraints backstop application rules
The application checks for invalid operations first, and SQLite constraints enforce one active session per user plus restricted deletes and references.

### Minimal API host owns composition
`Program.cs` wires the DbContext, repositories, handlers, validators, queries, Swagger, static files, and startup migrations. Lower layers do not own application composition.

---

## Naming Conventions

- Commands: `VerbNounCommand`
- Handlers: `VerbNounHandler`
- Repository interfaces: `I{Entity}Repository`
- Query interfaces: `I{Name}Query`
- Query implementations: `{Name}Query`
- DTOs: `{Name}Dto`
- Strongly typed IDs: `{Entity}Id`, plus `UserId`
- Error codes: centralized in `AppErrorCodes`, `AppWarningCodes`, and `DomainErrorCodes`

---

## File Placement Reference

| Type | Project | Namespace |
|------|---------|-----------|
| Aggregate roots and enums | `src/WorkTrace.Domain` | `WorkTrace.Domain.*` |
| Shared IDs, guards, domain exceptions | `src/WorkTrace.Domain/Shared` | `WorkTrace.Domain.Shared` |
| Commands, handlers, validation, app DTOs | `src/WorkTrace.Application` | `WorkTrace.Application.*` |
| Repository and runtime abstractions | `src/WorkTrace.Application/Abstractions` | `WorkTrace.Application.Abstractions` |
| EF Core DbContext, mappings, migrations | `src/WorkTrace.Infrastructure/Persistence` | `WorkTrace.Infrastructure.Persistence*` |
| Repository implementations | `src/WorkTrace.Infrastructure/Repositories` | `WorkTrace.Infrastructure.Repositories` |
| Unit of work and clock implementations | `src/WorkTrace.Infrastructure/UnitOfWork`, `Clock` | `WorkTrace.Infrastructure.*` |
| Read query DTOs and query services | `src/WorkTrace.ReadModel` | `WorkTrace.ReadModel.*` |
| Endpoint composition and host startup | `src/WorkTrace.Api` | top-level program |
| Unit and integration tests | `tests/*` | layer-specific test namespaces |

---

## Dependency Injection Pattern

- All service registration happens in `src/WorkTrace.Api/Program.cs`
- `AddDbContext<WorkTraceDbContext>` configures SQLite and falls back to `data/worktrace.db` when no connection string is provided
- `IClock` uses `SystemClock`
- `ICurrentUser` uses a fixed configured user id for MVP single-user operation
- `IUnitOfWork`, repository implementations, query services, handlers, and validators are registered explicitly as scoped services
- The API host applies EF Core migrations on startup
