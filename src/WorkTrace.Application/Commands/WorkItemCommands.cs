using WorkTrace.Domain.Shared;
using WorkTrace.Domain.WorkItems;

namespace WorkTrace.Application.Commands;

public sealed record CreateWorkItemCommand(string? Title, WorkItemKind Kind = WorkItemKind.Task, string? Description = null, ProjectId? ProjectId = null);

public sealed record UpdateWorkItemCommand(WorkItemId Id, string Title, WorkItemKind Kind, string? Description, ProjectId? ProjectId, WorkItemStatus Status);

public sealed record ArchiveWorkItemCommand(WorkItemId Id);

public sealed record RestoreWorkItemCommand(WorkItemId Id);
