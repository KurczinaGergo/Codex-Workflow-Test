# Architecture Reference

<!-- FILL IN: Living architecture document. Read by the implementation agent on demand.
     Rule: if any section exceeds 60 lines, extract it to docs/architecture-<topic>.md
     and replace it with a one-line pointer. Keep this file under 150 lines total.
     Do not duplicate content from AGENTS.md here — behavioral rules live there only. -->

---

## Layer Overview

<!-- FILL IN: Architectural layers and dependency rules. Include a diagram.

     Example (Clean Architecture):
     Domain ← Application ← Infrastructure ← API

     Rules:
     - Domain has zero external dependencies
     - Application depends only on Domain
     - Infrastructure implements interfaces defined in Domain/Application
     - API is thin: no business logic, only dispatch
-->

---

## Key Design Decisions

<!-- FILL IN: Significant design choices with reasoning.
     Keep each entry to 3–5 lines. Full rationale goes in an ADR.

     Example:
     ### Why CQRS?
     Each user action maps cleanly to a Command or Query.
     Full rationale: docs/decisions/ADR-001.md
-->

---

## Naming Conventions

<!-- FILL IN: Naming rules the implementation agent must follow.

     Example:
     - Commands: VerbNounCommand
     - Events: NounVerbedEvent (past tense)
     - Repositories: I{Entity}Repository
-->

---

## File Placement Reference

<!-- FILL IN: Class type → project and namespace mapping. -->

| Type | Project | Namespace |
|------|---------|-----------| 
| _add rows here_ | | |

---

## Dependency Injection Pattern

<!-- FILL IN: Who registers what, and where.

     Example:
     services.AddDomainServices();       // in Domain
     services.AddApplicationServices();  // in Application
     services.AddInfrastructureServices(configuration); // in Infrastructure
     All wired up in the API/host project only.
-->
