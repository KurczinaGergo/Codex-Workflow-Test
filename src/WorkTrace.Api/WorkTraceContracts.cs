namespace WorkTrace.Api;

public enum WorkItemStatus
{
    Todo = 0,
    InProgress = 1,
    Done = 2
}

public enum WorkItemKind
{
    Task = 0,
    Bug = 1
}

public enum NoteType
{
    Human = 0,
    System = 1
}

public sealed record WorkItemListItem(
    Guid Id,
    string Title,
    WorkItemKind Kind,
    string? Description,
    Guid? ProjectId,
    WorkItemStatus Status,
    bool IsArchived,
    DateTimeOffset? DoneAt,
    DateTimeOffset? ArchivedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    int SessionCount,
    int NoteCount);

public sealed record WorkSessionResponse(
    Guid Id,
    Guid UserId,
    Guid WorkItemId,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record NoteResponse(
    Guid Id,
    Guid WorkItemId,
    string Text,
    NoteType Type,
    DateTimeOffset CreatedAt,
    DateTimeOffset? EditedAt);

public sealed record WorkItemDetailResponse(
    Guid Id,
    string Title,
    WorkItemKind Kind,
    string? Description,
    Guid? ProjectId,
    WorkItemStatus Status,
    bool IsArchived,
    DateTimeOffset? DoneAt,
    DateTimeOffset? ArchivedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<WorkSessionResponse> Sessions,
    IReadOnlyList<NoteResponse> Notes);

public sealed record ProjectResponse(
    Guid Id,
    string Name,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record ActiveWorkResponse(
    Guid UserId,
    string UserName,
    WorkSessionResponse? ActiveSession,
    WorkItemListItem? ActiveWorkItem);

public sealed record TimelineEntryResponse(
    int Id,
    DateTimeOffset OccurredAt,
    string Kind,
    string Message,
    Guid? WorkItemId,
    Guid? WorkSessionId,
    Guid? NoteId,
    Guid? ProjectId);

public sealed record CreateWorkItemRequest(
    string? Title,
    WorkItemKind? Kind,
    string? Description,
    Guid? ProjectId);

public sealed record UpdateWorkItemStatusRequest(WorkItemStatus Status);

public sealed record StartWorkSessionRequest();

public sealed record StopWorkSessionRequest();

public sealed record CreateNoteRequest(
    string? Text,
    NoteType? Type);

public sealed record CreateProjectRequest(string? Name);

public sealed record RenameProjectRequest(string? Name);
