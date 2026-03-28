namespace WorkTrace.ReadModel.Dtos;

public sealed record NoteDto(
    Guid Id,
    string Text,
    string Type,
    DateTimeOffset CreatedAt,
    DateTimeOffset? EditedAt);
