using WorkTrace.Infrastructure.Persistence.Enums;

namespace WorkTrace.Infrastructure.Persistence.Entities;

public sealed class NoteRecord
{
    public Guid Id { get; set; }
    public Guid WorkItemId { get; set; }
    public WorkItemRecord? WorkItem { get; set; }
    public string Text { get; set; } = string.Empty;
    public NoteTypeRecord Type { get; set; } = NoteTypeRecord.Human;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? EditedAt { get; set; }
}
