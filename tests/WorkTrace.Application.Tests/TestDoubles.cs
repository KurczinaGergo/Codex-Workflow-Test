using WorkTrace.Application.Abstractions;
using WorkTrace.Application.Abstractions.Repositories;
using WorkTrace.Domain.Notes;
using WorkTrace.Domain.Projects;
using WorkTrace.Domain.Shared;
using WorkTrace.Domain.WorkItems;
using WorkTrace.Domain.WorkSessions;

namespace WorkTrace.Application.Tests;

internal sealed class FakeClock : IClock
{
    public DateTimeOffset UtcNow { get; set; }
}

internal sealed class FakeCurrentUser : ICurrentUser
{
    public UserId UserId { get; set; }
}

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveChangesCalls { get; private set; }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCalls++;
        return Task.CompletedTask;
    }
}

internal sealed class InMemoryWorkItemRepository : IWorkItemRepository
{
    private readonly Dictionary<WorkItemId, WorkItem> _items = new();

    public Task AddAsync(WorkItem workItem, CancellationToken cancellationToken = default)
    {
        _items[workItem.Id] = workItem;
        return Task.CompletedTask;
    }

    public Task<WorkItem?> GetByIdAsync(WorkItemId id, CancellationToken cancellationToken = default)
    {
        _items.TryGetValue(id, out var workItem);
        return Task.FromResult(workItem);
    }

    public Task<IReadOnlyList<WorkItem>> ListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<WorkItem>>(_items.Values.ToList());

    public Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken = default)
    {
        _items[workItem.Id] = workItem;
        return Task.CompletedTask;
    }
}

internal sealed class InMemoryWorkSessionRepository : IWorkSessionRepository
{
    private readonly Dictionary<WorkSessionId, WorkSession> _sessions = new();

    public Task AddAsync(WorkSession workSession, CancellationToken cancellationToken = default)
    {
        _sessions[workSession.Id] = workSession;
        return Task.CompletedTask;
    }

    public Task<WorkSession?> GetActiveByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var session = _sessions.Values.FirstOrDefault(item => item.UserId == userId && item.IsActive);
        return Task.FromResult(session);
    }

    public Task<WorkSession?> GetByIdAsync(WorkSessionId id, CancellationToken cancellationToken = default)
    {
        _sessions.TryGetValue(id, out var session);
        return Task.FromResult(session);
    }

    public Task<IReadOnlyList<WorkSession>> ListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<WorkSession>>(_sessions.Values.ToList());

    public Task UpdateAsync(WorkSession workSession, CancellationToken cancellationToken = default)
    {
        _sessions[workSession.Id] = workSession;
        return Task.CompletedTask;
    }
}

internal sealed class InMemoryNoteRepository : INoteRepository
{
    private readonly Dictionary<NoteId, Note> _notes = new();

    public Task AddAsync(Note note, CancellationToken cancellationToken = default)
    {
        _notes[note.Id] = note;
        return Task.CompletedTask;
    }

    public Task<Note?> GetByIdAsync(NoteId id, CancellationToken cancellationToken = default)
    {
        _notes.TryGetValue(id, out var note);
        return Task.FromResult(note);
    }

    public Task<IReadOnlyList<Note>> ListByWorkItemIdAsync(WorkItemId workItemId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Note>>(_notes.Values.Where(note => note.WorkItemId == workItemId).ToList());

    public Task UpdateAsync(Note note, CancellationToken cancellationToken = default)
    {
        _notes[note.Id] = note;
        return Task.CompletedTask;
    }
}

internal sealed class InMemoryProjectRepository : IProjectRepository
{
    private readonly Dictionary<ProjectId, Project> _projects = new();

    public Task AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        _projects[project.Id] = project;
        return Task.CompletedTask;
    }

    public Task<Project?> GetByIdAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        _projects.TryGetValue(id, out var project);
        return Task.FromResult(project);
    }

    public Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Project>>(_projects.Values.ToList());

    public Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        _projects[project.Id] = project;
        return Task.CompletedTask;
    }
}
