using Microsoft.EntityFrameworkCore;
using WorkTrace.Infrastructure.Abstractions;
using WorkTrace.Infrastructure.Persistence;
using WorkTrace.Infrastructure.Persistence.Entities;
using WorkTrace.ReadModel.Dtos;

namespace WorkTrace.ReadModel.Queries;

public sealed class ActiveWorkQuery : IActiveWorkQuery
{
    private readonly WorkTraceDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;

    public ActiveWorkQuery(WorkTraceDbContext db, ICurrentUser currentUser, IClock clock)
    {
        _db = db;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<ActiveWorkDto?> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var session = await _db.WorkSessions
            .AsNoTracking()
            .Include(x => x.WorkItem)
            .ThenInclude(x => x!.Project)
            .SingleOrDefaultAsync(x => x.UserId == _currentUser.UserId && x.EndedAt == null, cancellationToken);

        if (session is null || session.WorkItem is null)
        {
            return null;
        }

        return new ActiveWorkDto(
            session.Id,
            session.UserId,
            session.WorkItemId,
            session.WorkItem.Title,
            session.WorkItem.Kind.ToString(),
            session.WorkItem.Status.ToString(),
            session.WorkItem.ProjectId,
            session.WorkItem.Project?.Name,
            session.StartedAt,
            _clock.UtcNow - session.StartedAt);
    }
}
