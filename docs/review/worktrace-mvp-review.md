# WorkTrace MVP Review Bridge

## 1. Summary
This feature bootstraps the documented WorkTrace MVP into a buildable .NET 8 solution using the spec at [docs/specs/worktrace-mvp.md](/d:/Codex-Workflow-Test/docs/specs/worktrace-mvp.md). The workspace now includes domain aggregates, application commands and handlers, EF Core SQLite persistence, read-model queries, a static web UI, Minimal API endpoints, and tests across all layers. `dotnet test .\WorkTrace.sln` passes in the current environment. The main remaining issue is architectural convergence: the API currently uses an in-memory store instead of composing the Application, Infrastructure, and ReadModel layers.

## 2. Changed files
| File | Layer | Change |
|---|---|---|
| `src/WorkTrace.Domain/**` | Domain | Added IDs, guards, exceptions, enums, and aggregates |
| `src/WorkTrace.Application/**` | Application | Added abstractions, commands, handlers, validation, results, and query contracts |
| `src/WorkTrace.Infrastructure/**` | Infrastructure | Added EF Core persistence, repositories, adapters, unit of work, and DI |
| `src/WorkTrace.ReadModel/**` | ReadModel | Added DTOs, query interfaces, query implementations, and DI |
| `src/WorkTrace.Api/**` | API | Added Minimal API host, endpoint mapping, Swagger, and static UI |
| `tests/WorkTrace.*.Tests/**` | Tests | Added domain, application, infrastructure, and API coverage |
| `docs/specs/worktrace-mvp.md` | Docs | Added implementation spec |
| `docs/progress/features/worktrace-mvp.md` | Docs | Added feature progress record |
| `docs/progress/index.md` | Docs | Updated project progress and deviations |

## 3. Decisions
- What: Split the MVP into three implementation tracks.
  Why: It matched the repo workflow and kept the work parallelizable by layer responsibility.
  Alternative rejected: Building the entire stack serially in one pass.
- What: Used SQLite-backed EF Core tests for persistence and read models.
  Why: They validate the restrictive foreign keys and active-session uniqueness the docs call for.
  Alternative rejected: Pure in-memory repositories.
- What: Shipped the API as a functional in-memory host for MVP flows.
  Why: It allowed the API/UI track to complete independently while the lower layers were still being built.
  Alternative rejected: Blocking the API track entirely until integration was finished.

## 4. Critical excerpts
```csharp
entity.HasIndex(x => x.UserId).IsUnique().HasFilter("EndedAt IS NULL");
```
```csharp
var item = WorkItem.Create(command.Title, command.Kind, command.Description, command.ProjectId, _clock.UtcNow);
```
```csharp
builder.Services.AddSingleton<IWorkTraceStore, MvpWorkTraceStore>();
```

## 5. Spec deviations
- The API does not yet compose the Application, Infrastructure, and ReadModel layers; it serves the MVP through `MvpWorkTraceStore` in the API project.
- Infrastructure currently contains bootstrap `IClock`, `ICurrentUser`, and `IUnitOfWork` contracts even though the architecture doc places those abstractions in Application.
- The review-ready artifacts were first generated for the Infrastructure slice only and were later rewritten for the full feature.

## 6. Open questions
- Should the next pass replace `MvpWorkTraceStore` immediately, or is it acceptable as a temporary bootstrap host while the layered integration is wired?
- Should the duplicate runtime abstractions in Infrastructure be removed outright, or temporarily bridged to the existing Application contracts first?

## 7. Test coverage
- Domain tests cover work-item state transitions, done timestamp behavior, note validation, session stopping, and project rename rules.
- Application tests cover title auto-generation warnings, archived-item restoration, active-session conflicts, session stop promotion, note restoration, and project creation/rename flows.
- Infrastructure tests cover repository persistence, active-session uniqueness, restrictive delete behavior, and read-model query projections.
- API tests cover static UI hosting, health, work-item creation, note validation, session flow, Swagger availability, and unknown-route handling.
- Gap: There is no end-to-end test yet proving the API is backed by the layered Application/Infrastructure/ReadModel stack.

## 8. Proposed doc updates
- `docs/architecture.md` -> document the temporary API bootstrap store and duplicate runtime abstractions, or remove those deviations in a follow-up integration pass.
- `docs/progress/design-state.md` -> note that the current workspace is functionally complete but not yet fully aligned with the intended API composition model.
- New ADR required: yes. Draft: "During the WorkTrace MVP bootstrap, the API shipped with an in-memory `MvpWorkTraceStore` and Infrastructure temporarily duplicated runtime abstractions (`IClock`, `ICurrentUser`, `IUnitOfWork`) to let the three implementation tracks progress in parallel. This is a transitional state only; the intended end state remains a thin API composing Application, Infrastructure, and ReadModel services."
