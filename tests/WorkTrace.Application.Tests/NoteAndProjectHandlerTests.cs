using WorkTrace.Application.Commands;
using WorkTrace.Application.Handlers;
using WorkTrace.Application.Validation;
using WorkTrace.Domain.Notes;
using WorkTrace.Domain.Projects;
using WorkTrace.Domain.Shared;
using WorkTrace.Domain.WorkItems;

namespace WorkTrace.Application.Tests;

public class NoteAndProjectHandlerTests
{
    [Fact]
    public async Task AddNote_RestoresArchivedWorkItem()
    {
        var workItems = new InMemoryWorkItemRepository();
        var notes = new InMemoryNoteRepository();
        var clock = new FakeClock { UtcNow = new DateTimeOffset(2026, 03, 28, 12, 0, 0, TimeSpan.Zero) };
        var unitOfWork = new FakeUnitOfWork();
        var workItem = WorkItem.Create("Build", createdAt: clock.UtcNow.AddHours(-1));
        workItem.Archive(clock.UtcNow.AddMinutes(-5));
        await workItems.AddAsync(workItem);

        var handler = new AddNoteHandler(workItems, notes, clock, unitOfWork, new AddNoteValidator());
        var result = await handler.HandleAsync(new AddNoteCommand(workItem.Id, "Hello"));

        Assert.True(result.IsSuccess);
        Assert.False((await workItems.GetByIdAsync(workItem.Id))!.IsArchived);
        Assert.Single(result.Warnings);
        Assert.True(result.Value != NoteId.Empty);
    }

    [Fact]
    public async Task RenameProject_ChangesStoredName()
    {
        var projects = new InMemoryProjectRepository();
        var clock = new FakeClock { UtcNow = new DateTimeOffset(2026, 03, 28, 12, 0, 0, TimeSpan.Zero) };
        var unitOfWork = new FakeUnitOfWork();
        var project = Project.Create("Alpha", clock.UtcNow.AddHours(-1));
        await projects.AddAsync(project);

        var handler = new RenameProjectHandler(projects, clock, unitOfWork, new RenameProjectValidator());
        var result = await handler.HandleAsync(new RenameProjectCommand(project.Id, "Beta"));

        Assert.True(result.IsSuccess);
        Assert.Equal("Beta", (await projects.GetByIdAsync(project.Id))!.Name);
    }
}
