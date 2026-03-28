# Domain Model Reference

---

## Entity Overview

```text
Project (optional grouping)
  ^
  |
WorkItem
  |-- WorkSession[]
  `-- Note[]
```

- A `WorkItem` is the central referenceable unit of work
- A `WorkSession` always belongs to exactly one `WorkItem`
- A `Note` always belongs to exactly one `WorkItem`
- A `Project` is optional and never changes core WorkItem behavior

---

## Entities

### WorkItem
| Field | Type | Notes |
|------|------|-------|
| Id | `WorkItemId` | generated on create |
| Title | `string` | required; auto-generated when blank on create |
| Kind | `WorkItemKind` | required; defaults to `Task` |
| Description | `string?` | optional |
| ProjectId | `ProjectId?` | optional grouping |
| Status | `WorkItemStatus` | explicit user-facing state |
| DoneAt | `DateTimeOffset?` | set when entering `Done`, cleared when leaving it |
| IsArchived | `bool` | secondary state |
| ArchivedAt | `DateTimeOffset?` | set on archive, cleared on restore |
| CreatedAt | `DateTimeOffset` | immutable |
| UpdatedAt | `DateTimeOffset` | updates on meaningful mutation |

### WorkSession
| Field | Type | Notes |
|------|------|-------|
| Id | `WorkSessionId` | generated on create |
| UserId | `UserId` | MVP still records ownership explicitly |
| WorkItemId | `WorkItemId` | required |
| StartedAt | `DateTimeOffset` | immutable once created |
| EndedAt | `DateTimeOffset?` | null means active |
| CreatedAt | `DateTimeOffset` | immutable |
| UpdatedAt | `DateTimeOffset` | changes on stop or reassignment |

### Note
| Field | Type | Notes |
|------|------|-------|
| Id | `NoteId` | generated on create |
| WorkItemId | `WorkItemId` | required |
| Text | `string` | required, non-empty |
| Type | `NoteType` | `Human` or `System` |
| CreatedAt | `DateTimeOffset` | immutable |
| EditedAt | `DateTimeOffset?` | set only after a real text change |

### Project
| Field | Type | Notes |
|------|------|-------|
| Id | `ProjectId` | generated on create |
| Name | `string` | required, non-empty |
| CreatedAt | `DateTimeOffset` | immutable |
| UpdatedAt | `DateTimeOffset` | changes on rename |

---

## Enumerations

### WorkItemStatus
`Todo -> InProgress -> Done`

- All transitions are allowed
- Re-entering `Done` refreshes `DoneAt`
- Leaving `Done` clears `DoneAt`

### WorkItemKind
- `Task`
- `Bug`

Kind is classification only and has no behavioral effect in the MVP.

### NoteType
- `Human`
- `System`

---

## Key Invariants

- WorkItem title must be non-empty after creation
- WorkItem kind and status must be valid enum values
- WorkSession duration must be positive; zero or negative durations are invalid
- A stopped WorkSession cannot be stopped again
- A user can have only one active WorkSession at a time; application logic and DB constraints both enforce this
- Notes must have non-empty text
- Notes do not change WorkItem status
- Project assignment is optional and removable
- Archive is secondary state, separate from WorkItem status
- Auto-promotion to `InProgress` happens only after valid recorded time, not just timer start
- Logging more time on a `Done` WorkItem refreshes `DoneAt`
- New session, note, or meaningful edit may restore an archived WorkItem through application orchestration
