namespace WorkTrace.Infrastructure.Persistence.Entities;

public sealed class WorkSessionRecord
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid WorkItemId { get; set; }
    public WorkItemRecord? WorkItem { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
