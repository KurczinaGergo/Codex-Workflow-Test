using WorkTrace.Domain.Shared;
using WorkTrace.Domain.Projects;
using WorkTrace.Domain.WorkItems;

namespace WorkTrace.Domain.Tests;

public class WorkItemTests
{
    [Fact]
    public void Create_UsesDefaultTitle_WhenTitleIsBlank()
    {
        var createdAt = new DateTimeOffset(2026, 03, 28, 12, 0, 0, TimeSpan.Zero);

        var workItem = WorkItem.Create("   ", WorkItemKind.Task, null, null, createdAt);

        Assert.Equal("Untitled work item", workItem.Title);
        Assert.Equal(WorkItemStatus.Todo, workItem.Status);
        Assert.Equal(createdAt, workItem.CreatedAt);
        Assert.Equal(createdAt, workItem.UpdatedAt);
    }

    [Fact]
    public void SetStatus_ToDone_RefreshesDoneAt()
    {
        var workItem = WorkItem.Create("Build", createdAt: DateTimeOffset.UtcNow);
        var doneAt = new DateTimeOffset(2026, 03, 28, 13, 0, 0, TimeSpan.Zero);
        var refreshedAt = doneAt.AddMinutes(20);

        workItem.SetStatus(WorkItemStatus.Done, doneAt);
        workItem.SetStatus(WorkItemStatus.Done, refreshedAt);

        Assert.Equal(WorkItemStatus.Done, workItem.Status);
        Assert.Equal(refreshedAt, workItem.DoneAt);
    }

    [Fact]
    public void SetStatus_AwayFromDone_ClearsDoneAt()
    {
        var workItem = WorkItem.Create("Build", createdAt: DateTimeOffset.UtcNow);
        var doneAt = new DateTimeOffset(2026, 03, 28, 13, 0, 0, TimeSpan.Zero);
        var later = doneAt.AddMinutes(15);

        workItem.SetStatus(WorkItemStatus.Done, doneAt);
        workItem.SetStatus(WorkItemStatus.InProgress, later);

        Assert.Equal(WorkItemStatus.InProgress, workItem.Status);
        Assert.Null(workItem.DoneAt);
    }

    [Fact]
    public void Archive_And_Restore_UpdateState()
    {
        var workItem = WorkItem.Create("Build", createdAt: DateTimeOffset.UtcNow);
        var archivedAt = new DateTimeOffset(2026, 03, 28, 14, 0, 0, TimeSpan.Zero);
        var restoredAt = archivedAt.AddMinutes(10);

        workItem.Archive(archivedAt);
        workItem.Restore(restoredAt);

        Assert.False(workItem.IsArchived);
        Assert.Null(workItem.ArchivedAt);
        Assert.Equal(restoredAt, workItem.UpdatedAt);
    }
}
