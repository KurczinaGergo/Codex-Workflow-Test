namespace WorkTrace.ReadModel.Dtos;

public sealed record TimelineEntryDto(
    Guid? WorkSessionId,
    Guid? NoteId,
    string EntryType,
    DateTimeOffset OccurredAt,
    string Summary);
