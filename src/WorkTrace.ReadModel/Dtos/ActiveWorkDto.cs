namespace WorkTrace.ReadModel.Dtos;

public sealed record ActiveWorkDto(
    Guid WorkSessionId,
    string UserId,
    Guid WorkItemId,
    string WorkItemTitle,
    string WorkItemKind,
    string WorkItemStatus,
    Guid? ProjectId,
    string? ProjectName,
    DateTimeOffset StartedAt,
    TimeSpan Elapsed);
