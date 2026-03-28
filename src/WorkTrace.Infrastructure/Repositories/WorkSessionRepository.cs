using Microsoft.EntityFrameworkCore;
using WorkTrace.Infrastructure.Persistence;
using WorkTrace.Infrastructure.Persistence.Entities;

namespace WorkTrace.Infrastructure.Repositories;

public sealed class WorkSessionRepository
{
    private readonly WorkTraceDbContext _db;

    public WorkSessionRepository(WorkTraceDbContext db)
    {
        _db = db;
    }

    public Task<WorkSessionRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.WorkSessions
            .Include(x => x.WorkItem)
            .ThenInclude(x => x!.Project)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<WorkSessionRecord?> GetActiveByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        => _db.WorkSessions
            .Include(x => x.WorkItem)
            .ThenInclude(x => x!.Project)
            .SingleOrDefaultAsync(x => x.UserId == userId && x.EndedAt == null, cancellationToken);

    public Task<List<WorkSessionRecord>> ListByWorkItemIdAsync(Guid workItemId, CancellationToken cancellationToken = default)
        => _db.WorkSessions
            .AsNoTracking()
            .Where(x => x.WorkItemId == workItemId)
            .ToListAsync(cancellationToken);

    public Task AddAsync(WorkSessionRecord workSession, CancellationToken cancellationToken = default)
        => _db.WorkSessions.AddAsync(workSession, cancellationToken).AsTask();

    public void Remove(WorkSessionRecord workSession) => _db.WorkSessions.Remove(workSession);
}
