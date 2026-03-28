using WorkTrace.Domain.Shared;

namespace WorkTrace.Domain.WorkItems;

public sealed class WorkItem
{
    private const string DefaultTitle = "Untitled work item";

    private WorkItem(
        WorkItemId id,
        string title,
        WorkItemKind kind,
        string? description,
        ProjectId? projectId,
        WorkItemStatus status,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        Title = title;
        Kind = kind;
        Description = description;
        ProjectId = projectId;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public WorkItemId Id { get; }

    public string Title { get; private set; }

    public WorkItemKind Kind { get; private set; }

    public string? Description { get; private set; }

    public ProjectId? ProjectId { get; private set; }

    public WorkItemStatus Status { get; private set; }

    public DateTimeOffset? DoneAt { get; private set; }

    public bool IsArchived { get; private set; }

    public DateTimeOffset? ArchivedAt { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static WorkItem Create(
        string? title,
        WorkItemKind kind = WorkItemKind.Task,
        string? description = null,
        ProjectId? projectId = null,
        DateTimeOffset? createdAt = null)
    {
        Guard.AgainstInvalidEnum(kind, nameof(kind), "Work item kind is invalid.");

        var now = createdAt ?? DateTimeOffset.UtcNow;
        var normalizedTitle = string.IsNullOrWhiteSpace(title) ? DefaultTitle : title.Trim();

        return new WorkItem(
            WorkItemId.New(),
            normalizedTitle,
            kind,
            NormalizeOptional(description),
            projectId,
            WorkItemStatus.Todo,
            now,
            now);
    }

    public void UpdateDetails(
        string title,
        WorkItemKind kind,
        string? description,
        ProjectId? projectId,
        WorkItemStatus status,
        DateTimeOffset updatedAt)
    {
        var normalizedTitle = Guard.AgainstNullOrWhiteSpace(title, nameof(title), "Work item title is required.");
        Guard.AgainstInvalidEnum(kind, nameof(kind), "Work item kind is invalid.");
        Guard.AgainstInvalidEnum(status, nameof(status), "Work item status is invalid.");

        var changed = false;

        if (Title != normalizedTitle)
        {
            Title = normalizedTitle;
            changed = true;
        }

        if (Kind != kind)
        {
            Kind = kind;
            changed = true;
        }

        var normalizedDescription = NormalizeOptional(description);
        if (Description != normalizedDescription)
        {
            Description = normalizedDescription;
            changed = true;
        }

        if (ProjectId != projectId)
        {
            ProjectId = projectId;
            changed = true;
        }

        if (status != Status || status == WorkItemStatus.Done)
        {
            ApplyStatus(status, updatedAt);
            changed = true;
        }

        if (changed)
        {
            UpdatedAt = updatedAt;
        }
    }

    public void SetStatus(WorkItemStatus status, DateTimeOffset updatedAt)
    {
        Guard.AgainstInvalidEnum(status, nameof(status), "Work item status is invalid.");
        if (status == Status && status != WorkItemStatus.Done)
        {
            return;
        }

        ApplyStatus(status, updatedAt);
        UpdatedAt = updatedAt;
    }

    public void Archive(DateTimeOffset archivedAt)
    {
        if (IsArchived)
        {
            return;
        }

        IsArchived = true;
        ArchivedAt = archivedAt;
        UpdatedAt = archivedAt;
    }

    public void Restore(DateTimeOffset restoredAt)
    {
        if (!IsArchived)
        {
            return;
        }

        IsArchived = false;
        ArchivedAt = null;
        UpdatedAt = restoredAt;
    }

    private void ApplyStatus(WorkItemStatus status, DateTimeOffset updatedAt)
    {
        Status = status;
        DoneAt = status == WorkItemStatus.Done ? updatedAt : null;
    }

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}
