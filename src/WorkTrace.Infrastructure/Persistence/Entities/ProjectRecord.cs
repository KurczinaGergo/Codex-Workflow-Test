namespace WorkTrace.Infrastructure.Persistence.Entities;

public sealed class ProjectRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<WorkItemRecord> WorkItems { get; } = new List<WorkItemRecord>();
}
