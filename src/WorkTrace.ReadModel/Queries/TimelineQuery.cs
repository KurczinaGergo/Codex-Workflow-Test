using Microsoft.EntityFrameworkCore;
using WorkTrace.Infrastructure.Persistence;
using WorkTrace.ReadModel.Dtos;

namespace WorkTrace.ReadModel.Queries;

public sealed class TimelineQuery : ITimelineQuery
{
    private readonly WorkTraceDbContext _db;

    public TimelineQuery(WorkTraceDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TimelineEntryDto>> GetAsync(Guid workItemId, CancellationToken cancellationToken = default)
    {
        var sessions = await _db.WorkSessions
            .AsNoTracking()
            .Where(x => x.WorkItemId == workItemId)
            .ToListAsync(cancellationToken);

        var notes = await _db.Notes
            .AsNoTracking()
            .Where(x => x.WorkItemId == workItemId)
            .ToListAsync(cancellationToken);

        var timeline = sessions.SelectMany(session =>
                new TimelineEntryDto?[]
                {
                    new TimelineEntryDto(
                        session.Id,
                        null,
                        "WorkSessionStarted",
                        session.StartedAt,
                        $"Session started by {session.UserId}"),
                    session.EndedAt is null
                        ? null
                        : new TimelineEntryDto(
                            session.Id,
                            null,
                            "WorkSessionEnded",
                            session.EndedAt.Value,
                            $"Session ended by {session.UserId}")
                }.OfType<TimelineEntryDto>())
            .Concat(notes.SelectMany(note =>
                new TimelineEntryDto?[]
                {
                    new TimelineEntryDto(
                        null,
                        note.Id,
                        "NoteAdded",
                        note.CreatedAt,
                        note.Text),
                    note.EditedAt is null
                        ? null
                        : new TimelineEntryDto(
                            null,
                            note.Id,
                            "NoteEdited",
                            note.EditedAt.Value,
                            note.Text)
                }.OfType<TimelineEntryDto>()))
            .OrderBy(entry => entry.OccurredAt)
            .ToList();

        return timeline;
    }
}
