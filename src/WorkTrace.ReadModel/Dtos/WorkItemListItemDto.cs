namespace WorkTrace.ReadModel.Dtos;

public sealed record WorkItemListItemDto(
    Guid Id,
    string Title,
    string Kind,
    string Status,
    Guid? ProjectId,
    string? ProjectName,
    bool IsArchived,
    DateTimeOffset? DoneAt,
    DateTimeOffset? ArchivedAt,
    DateTimeOffset UpdatedAt,
    int SessionCount,
    int NoteCount);
