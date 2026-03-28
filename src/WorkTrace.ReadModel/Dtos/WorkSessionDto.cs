namespace WorkTrace.ReadModel.Dtos;

public sealed record WorkSessionDto(
    Guid Id,
    string UserId,
    Guid WorkItemId,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt,
    bool IsActive,
    TimeSpan Duration);
