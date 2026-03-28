namespace WorkTrace.ReadModel.Dtos;

public sealed record WorkItemDetailDto(
    Guid Id,
    string Title,
    string Kind,
    string Status,
    string? Description,
    Guid? ProjectId,
    string? ProjectName,
    bool IsArchived,
    DateTimeOffset? DoneAt,
    DateTimeOffset? ArchivedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<WorkSessionDto> Sessions,
    IReadOnlyList<NoteDto> Notes);
