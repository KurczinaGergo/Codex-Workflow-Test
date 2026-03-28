using WorkTrace.Domain.Shared;

namespace WorkTrace.Application.Commands;

public sealed record StartWorkSessionCommand(WorkItemId WorkItemId);

public sealed record StopWorkSessionCommand(WorkSessionId Id);

public sealed record ReassignWorkSessionCommand(WorkSessionId Id, WorkItemId WorkItemId);
