using Microsoft.Extensions.Options;

namespace WorkTrace.Api;

public interface IWorkTraceStore
{
    IReadOnlyList<WorkItemListItem> GetWorkItems();
    WorkItemDetailResponse GetWorkItem(Guid id);
    WorkItemDetailResponse CreateWorkItem(CreateWorkItemRequest request);
    WorkItemDetailResponse UpdateWorkItemStatus(Guid id, UpdateWorkItemStatusRequest request);
    WorkItemDetailResponse ArchiveWorkItem(Guid id);
    WorkItemDetailResponse RestoreWorkItem(Guid id);
    WorkSessionResponse StartWorkSession(Guid workItemId);
    WorkSessionResponse StopWorkSession(Guid workSessionId);
    NoteResponse AddNote(Guid workItemId, CreateNoteRequest request);
    IReadOnlyList<ProjectResponse> GetProjects();
    ProjectResponse CreateProject(CreateProjectRequest request);
    ProjectResponse RenameProject(Guid projectId, RenameProjectRequest request);
    ActiveWorkResponse GetActiveWork();
    IReadOnlyList<TimelineEntryResponse> GetTimeline();
}

public sealed class MvpWorkTraceStore : IWorkTraceStore
{
    private const string DefaultTitlePrefix = "Untitled work item";

    private readonly object _gate = new();
    private readonly IClock _clock;
    private readonly WorkTraceOptions _options;
    private readonly List<WorkItemRecord> _workItems = new();
    private readonly List<ProjectRecord> _projects = new();
    private readonly List<TimelineEntryRecord> _timeline = new();
    private readonly Dictionary<Guid, WorkSessionRecord> _sessions = new();
    private readonly Dictionary<Guid, NoteRecord> _notes = new();
    private int _timelineCounter = 1;
    private int _autoTitleCounter = 1;

    public MvpWorkTraceStore(IClock clock, IOptions<WorkTraceOptions> options)
    {
        _clock = clock;
        _options = options.Value;
        Seed();
    }

    public IReadOnlyList<WorkItemListItem> GetWorkItems()
    {
        lock (_gate)
        {
            return _workItems
                .OrderByDescending(workItem => workItem.UpdatedAt)
                .Select(MapListItem)
                .ToList();
        }
    }

    public WorkItemDetailResponse GetWorkItem(Guid id)
    {
        lock (_gate)
        {
            return MapDetail(GetRequiredWorkItem(id));
        }
    }

    public WorkItemDetailResponse CreateWorkItem(CreateWorkItemRequest request)
    {
        lock (_gate)
        {
            var now = _clock.UtcNow;
            var title = NormalizeTitle(request.Title);
            var kind = request.Kind ?? WorkItemKind.Task;
            var projectId = request.ProjectId;

            if (projectId.HasValue && _projects.All(project => project.Id != projectId.Value))
            {
                throw new ApiValidationException("Project does not exist.");
            }

            var record = new WorkItemRecord
            {
                Id = Guid.NewGuid(),
                Title = title,
                Kind = kind,
                Description = NormalizeOptionalText(request.Description),
                ProjectId = projectId,
                Status = WorkItemStatus.Todo,
                CreatedAt = now,
                UpdatedAt = now
            };

            _workItems.Add(record);
            AddTimeline("work-item-created", $"Created work item '{record.Title}'.", now, record.Id, null, null, null);
            return MapDetail(record);
        }
    }

    public WorkItemDetailResponse UpdateWorkItemStatus(Guid id, UpdateWorkItemStatusRequest request)
    {
        lock (_gate)
        {
            var workItem = GetRequiredWorkItem(id);
            var now = _clock.UtcNow;

            if (workItem.Status != request.Status)
            {
                workItem.Status = request.Status;
                workItem.UpdatedAt = now;
                if (request.Status == WorkItemStatus.Done)
                {
                    workItem.DoneAt = now;
                }
                else if (workItem.DoneAt.HasValue)
                {
                    workItem.DoneAt = null;
                }
                AddTimeline("work-item-status-changed", $"Set work item '{workItem.Title}' to {request.Status}.", now, workItem.Id, null, null, null);
            }

            return MapDetail(workItem);
        }
    }

    public WorkItemDetailResponse ArchiveWorkItem(Guid id)
    {
        lock (_gate)
        {
            var workItem = GetRequiredWorkItem(id);
            if (!workItem.IsArchived)
            {
                var now = _clock.UtcNow;
                workItem.IsArchived = true;
                workItem.ArchivedAt = now;
                workItem.UpdatedAt = now;
                AddTimeline("work-item-archived", $"Archived work item '{workItem.Title}'.", now, workItem.Id, null, null, null);
            }

            return MapDetail(workItem);
        }
    }

    public WorkItemDetailResponse RestoreWorkItem(Guid id)
    {
        lock (_gate)
        {
            var workItem = GetRequiredWorkItem(id);
            if (workItem.IsArchived)
            {
                var now = _clock.UtcNow;
                workItem.IsArchived = false;
                workItem.ArchivedAt = null;
                workItem.UpdatedAt = now;
                AddTimeline("work-item-restored", $"Restored work item '{workItem.Title}'.", now, workItem.Id, null, null, null);
            }

            return MapDetail(workItem);
        }
    }

    public WorkSessionResponse StartWorkSession(Guid workItemId)
    {
        lock (_gate)
        {
            var workItem = GetRequiredWorkItem(workItemId);
            if (workItem.IsArchived)
            {
                RestoreArchivedWorkItem(workItem);
            }

            if (_sessions.Values.Any(session => session.UserId == _options.CurrentUserId && session.EndedAt is null))
            {
                throw new ApiConflictException("There is already an active work session for the current user.");
            }

            var now = _clock.UtcNow;
            var record = new WorkSessionRecord
            {
                Id = Guid.NewGuid(),
                UserId = _options.CurrentUserId,
                WorkItemId = workItem.Id,
                StartedAt = now,
                CreatedAt = now,
                UpdatedAt = now
            };

            _sessions.Add(record.Id, record);
            workItem.SessionIds.Add(record.Id);
            AddTimeline("work-session-started", $"Started a work session on '{workItem.Title}'.", now, workItem.Id, record.Id, null, null);
            return MapSession(record);
        }
    }

    public WorkSessionResponse StopWorkSession(Guid workSessionId)
    {
        lock (_gate)
        {
            if (!_sessions.TryGetValue(workSessionId, out var session))
            {
                throw new ApiNotFoundException("Work session was not found.");
            }

            if (session.EndedAt is not null)
            {
                throw new ApiConflictException("Work session has already been stopped.");
            }

            var now = _clock.UtcNow;
            if (now <= session.StartedAt)
            {
                throw new ApiValidationException("Work session stop time must be after the start time.");
            }

            session.EndedAt = now;
            session.UpdatedAt = now;

            var workItem = GetRequiredWorkItem(session.WorkItemId);
            if (workItem.IsArchived)
            {
                RestoreArchivedWorkItem(workItem);
            }

            if (workItem.Status == WorkItemStatus.Todo)
            {
                workItem.Status = WorkItemStatus.InProgress;
                workItem.UpdatedAt = now;
                AddTimeline("work-item-promoted", $"Promoted work item '{workItem.Title}' to InProgress after tracking time.", now, workItem.Id, session.Id, null, null);
            }

            if (workItem.Status == WorkItemStatus.Done)
            {
                workItem.DoneAt = now;
                workItem.UpdatedAt = now;
                AddTimeline("work-item-done-refreshed", $"Refreshed DoneAt for '{workItem.Title}' after tracking time.", now, workItem.Id, session.Id, null, null);
            }

            AddTimeline("work-session-stopped", $"Stopped work session on '{workItem.Title}'.", now, workItem.Id, session.Id, null, null);
            return MapSession(session);
        }
    }

    public NoteResponse AddNote(Guid workItemId, CreateNoteRequest request)
    {
        lock (_gate)
        {
            var workItem = GetRequiredWorkItem(workItemId);
            var now = _clock.UtcNow;
            var text = NormalizeRequiredText(request.Text, "Note text is required.");
            var type = request.Type ?? NoteType.Human;

            if (workItem.IsArchived)
            {
                RestoreArchivedWorkItem(workItem);
            }

            var record = new NoteRecord
            {
                Id = Guid.NewGuid(),
                WorkItemId = workItem.Id,
                Text = text,
                Type = type,
                CreatedAt = now
            };

            _notes.Add(record.Id, record);
            workItem.NoteIds.Add(record.Id);
            workItem.UpdatedAt = now;
            AddTimeline("note-added", $"Added a note to '{workItem.Title}'.", now, workItem.Id, null, record.Id, null);
            return MapNote(record);
        }
    }

    public IReadOnlyList<ProjectResponse> GetProjects()
    {
        lock (_gate)
        {
            return _projects
                .OrderBy(project => project.Name)
                .Select(MapProject)
                .ToList();
        }
    }

    public ProjectResponse CreateProject(CreateProjectRequest request)
    {
        lock (_gate)
        {
            var now = _clock.UtcNow;
            var name = NormalizeRequiredText(request.Name, "Project name is required.");
            if (_projects.Any(project => project.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ApiConflictException("A project with that name already exists.");
            }

            var record = new ProjectRecord
            {
                Id = Guid.NewGuid(),
                Name = name,
                CreatedAt = now,
                UpdatedAt = now
            };

            _projects.Add(record);
            AddTimeline("project-created", $"Created project '{record.Name}'.", now, null, null, null, record.Id);
            return MapProject(record);
        }
    }

    public ProjectResponse RenameProject(Guid projectId, RenameProjectRequest request)
    {
        lock (_gate)
        {
            var project = GetRequiredProject(projectId);
            var now = _clock.UtcNow;
            var name = NormalizeRequiredText(request.Name, "Project name is required.");
            if (_projects.Any(candidate => candidate.Id != project.Id && candidate.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ApiConflictException("A project with that name already exists.");
            }

            project.Name = name;
            project.UpdatedAt = now;
            AddTimeline("project-renamed", $"Renamed project to '{project.Name}'.", now, null, null, null, project.Id);
            return MapProject(project);
        }
    }

    public ActiveWorkResponse GetActiveWork()
    {
        lock (_gate)
        {
            var activeSession = _sessions.Values.FirstOrDefault(session => session.UserId == _options.CurrentUserId && session.EndedAt is null);
            if (activeSession is null)
            {
                return new ActiveWorkResponse(_options.CurrentUserId, _options.CurrentUserName, null, null);
            }

            var workItem = GetRequiredWorkItem(activeSession.WorkItemId);
            return new ActiveWorkResponse(
                _options.CurrentUserId,
                _options.CurrentUserName,
                MapSession(activeSession),
                MapListItem(workItem));
        }
    }

    public IReadOnlyList<TimelineEntryResponse> GetTimeline()
    {
        lock (_gate)
        {
            return _timeline
                .OrderByDescending(entry => entry.OccurredAt)
                .Select(MapTimeline)
                .Take(50)
                .ToList();
        }
    }

    private void Seed()
    {
        var now = _clock.UtcNow;
        var personalProject = new ProjectRecord
        {
            Id = Guid.NewGuid(),
            Name = "Personal",
            CreatedAt = now,
            UpdatedAt = now
        };
        _projects.Add(personalProject);

        var workItem = new WorkItemRecord
        {
            Id = Guid.NewGuid(),
            Title = "Set up WorkTrace MVP",
            Kind = WorkItemKind.Task,
            Description = "Baseline item used to seed the API demo workspace.",
            ProjectId = personalProject.Id,
            Status = WorkItemStatus.InProgress,
            CreatedAt = now.AddMinutes(-25),
            UpdatedAt = now.AddMinutes(-10)
        };

        _workItems.Add(workItem);
        AddTimeline("seeded", "Seeded the demo workspace.", now.AddMinutes(-25), workItem.Id, null, null, personalProject.Id);
    }

    private WorkItemRecord GetRequiredWorkItem(Guid id) =>
        _workItems.FirstOrDefault(workItem => workItem.Id == id)
        ?? throw new ApiNotFoundException("Work item was not found.");

    private ProjectRecord GetRequiredProject(Guid id) =>
        _projects.FirstOrDefault(project => project.Id == id)
        ?? throw new ApiNotFoundException("Project was not found.");

    private string NormalizeTitle(string? title)
    {
        var normalized = NormalizeOptionalText(title);
        return string.IsNullOrWhiteSpace(normalized)
            ? $"{DefaultTitlePrefix} {_autoTitleCounter++}"
            : normalized;
    }

    private static string? NormalizeOptionalText(string? text)
    {
        var normalized = text?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    private static string NormalizeRequiredText(string? text, string errorMessage)
    {
        var normalized = text?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ApiValidationException(errorMessage);
        }

        return normalized;
    }

    private void RestoreArchivedWorkItem(WorkItemRecord workItem)
    {
        if (!workItem.IsArchived)
        {
            return;
        }

        var now = _clock.UtcNow;
        workItem.IsArchived = false;
        workItem.ArchivedAt = null;
        workItem.UpdatedAt = now;
        AddTimeline("work-item-restored", $"Restored work item '{workItem.Title}'.", now, workItem.Id, null, null, null);
    }

    private void AddTimeline(string kind, string message, DateTimeOffset occurredAt, Guid? workItemId, Guid? workSessionId, Guid? noteId, Guid? projectId)
    {
        _timeline.Add(new TimelineEntryRecord
        {
            Id = _timelineCounter++,
            OccurredAt = occurredAt,
            Kind = kind,
            Message = message,
            WorkItemId = workItemId,
            WorkSessionId = workSessionId,
            NoteId = noteId,
            ProjectId = projectId
        });
    }

    private WorkItemListItem MapListItem(WorkItemRecord record) =>
        new(
            record.Id,
            record.Title,
            record.Kind,
            record.Description,
            record.ProjectId,
            record.Status,
            record.IsArchived,
            record.DoneAt,
            record.ArchivedAt,
            record.CreatedAt,
            record.UpdatedAt,
            record.SessionIds.Count,
            record.NoteIds.Count);

    private WorkItemDetailResponse MapDetail(WorkItemRecord record) =>
        new(
            record.Id,
            record.Title,
            record.Kind,
            record.Description,
            record.ProjectId,
            record.Status,
            record.IsArchived,
            record.DoneAt,
            record.ArchivedAt,
            record.CreatedAt,
            record.UpdatedAt,
            record.SessionIds.Select(id => MapSession(_sessions[id])).OrderByDescending(session => session.StartedAt).ToList(),
            record.NoteIds.Select(id => MapNote(_notes[id])).OrderByDescending(note => note.CreatedAt).ToList());

    private WorkSessionResponse MapSession(WorkSessionRecord record) =>
        new(record.Id, record.UserId, record.WorkItemId, record.StartedAt, record.EndedAt, record.CreatedAt, record.UpdatedAt);

    private NoteResponse MapNote(NoteRecord record) =>
        new(record.Id, record.WorkItemId, record.Text, record.Type, record.CreatedAt, record.EditedAt);

    private ProjectResponse MapProject(ProjectRecord record) =>
        new(record.Id, record.Name, record.CreatedAt, record.UpdatedAt);

    private ActiveWorkResponse MapActiveWork(WorkSessionRecord record)
    {
        var workItem = GetRequiredWorkItem(record.WorkItemId);
        return new ActiveWorkResponse(_options.CurrentUserId, _options.CurrentUserName, MapSession(record), MapListItem(workItem));
    }

    private TimelineEntryResponse MapTimeline(TimelineEntryRecord record) =>
        new(record.Id, record.OccurredAt, record.Kind, record.Message, record.WorkItemId, record.WorkSessionId, record.NoteId, record.ProjectId);

    private sealed class WorkItemRecord
    {
        public Guid Id { get; init; }

        public string Title { get; set; } = string.Empty;

        public WorkItemKind Kind { get; set; }

        public string? Description { get; set; }

        public Guid? ProjectId { get; set; }

        public WorkItemStatus Status { get; set; }

        public bool IsArchived { get; set; }

        public DateTimeOffset? DoneAt { get; set; }

        public DateTimeOffset? ArchivedAt { get; set; }

        public DateTimeOffset CreatedAt { get; init; }

        public DateTimeOffset UpdatedAt { get; set; }

        public List<Guid> SessionIds { get; } = new();

        public List<Guid> NoteIds { get; } = new();
    }

    private sealed class WorkSessionRecord
    {
        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public Guid WorkItemId { get; init; }

        public DateTimeOffset StartedAt { get; init; }

        public DateTimeOffset? EndedAt { get; set; }

        public DateTimeOffset CreatedAt { get; init; }

        public DateTimeOffset UpdatedAt { get; set; }
    }

    private sealed class NoteRecord
    {
        public Guid Id { get; init; }

        public Guid WorkItemId { get; init; }

        public string Text { get; set; } = string.Empty;

        public NoteType Type { get; set; }

        public DateTimeOffset CreatedAt { get; init; }

        public DateTimeOffset? EditedAt { get; set; }
    }

    private sealed class ProjectRecord
    {
        public Guid Id { get; init; }

        public string Name { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; init; }

        public DateTimeOffset UpdatedAt { get; set; }
    }

    private sealed class TimelineEntryRecord
    {
        public int Id { get; init; }

        public DateTimeOffset OccurredAt { get; init; }

        public string Kind { get; init; } = string.Empty;

        public string Message { get; init; } = string.Empty;

        public Guid? WorkItemId { get; init; }

        public Guid? WorkSessionId { get; init; }

        public Guid? NoteId { get; init; }

        public Guid? ProjectId { get; init; }
    }
}
