using WorkTrace.Domain.Shared;

namespace WorkTrace.Domain.Notes;

public sealed class Note
{
    private Note(
        NoteId id,
        WorkItemId workItemId,
        string text,
        NoteType type,
        DateTimeOffset createdAt)
    {
        Id = id;
        WorkItemId = workItemId;
        Text = text;
        Type = type;
        CreatedAt = createdAt;
    }

    public NoteId Id { get; }

    public WorkItemId WorkItemId { get; private set; }

    public string Text { get; private set; }

    public NoteType Type { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? EditedAt { get; private set; }

    public static Note Create(WorkItemId workItemId, string text, NoteType type, DateTimeOffset createdAt)
    {
        EnsureWorkItemId(workItemId);
        Guard.AgainstInvalidEnum(type, nameof(type), "Note type is invalid.");

        return new Note(
            NoteId.New(),
            workItemId,
            Guard.AgainstNullOrWhiteSpace(text, nameof(text), "Note text is required."),
            type,
            createdAt);
    }

    public void EditText(string text, DateTimeOffset editedAt)
    {
        var normalized = Guard.AgainstNullOrWhiteSpace(text, nameof(text), "Note text is required.");

        if (Text != normalized)
        {
            Text = normalized;
            EditedAt = editedAt;
        }
    }

    private static void EnsureWorkItemId(WorkItemId workItemId)
    {
        if (workItemId == WorkItemId.Empty)
        {
            throw new DomainException(DomainErrorCodes.RequiredValue, nameof(workItemId), "A work item id is required.");
        }
    }
}
