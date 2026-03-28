using WorkTrace.Infrastructure.Persistence.Enums;

namespace WorkTrace.Infrastructure.Persistence.Entities;

public sealed class WorkItemRecord
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public WorkItemKindRecord Kind { get; set; } = WorkItemKindRecord.Task;
    public string? Description { get; set; }
    public Guid? ProjectId { get; set; }
    public ProjectRecord? Project { get; set; }
    public WorkItemStatusRecord Status { get; set; } = WorkItemStatusRecord.Todo;
    public DateTimeOffset? DoneAt { get; set; }
    public bool IsArchived { get; set; }
    public DateTimeOffset? ArchivedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<WorkSessionRecord> WorkSessions { get; } = new List<WorkSessionRecord>();
    public ICollection<NoteRecord> Notes { get; } = new List<NoteRecord>();
}
