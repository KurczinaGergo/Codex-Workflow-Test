using Microsoft.EntityFrameworkCore;
using WorkTrace.Infrastructure.Persistence;
using WorkTrace.Infrastructure.Persistence.Entities;

namespace WorkTrace.Infrastructure.Repositories;

public sealed class WorkItemRepository
{
    private readonly WorkTraceDbContext _db;

    public WorkItemRepository(WorkTraceDbContext db)
    {
        _db = db;
    }

    public Task<WorkItemRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.WorkItems
            .Include(x => x.Project)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<List<WorkItemRecord>> ListAsync(CancellationToken cancellationToken = default)
        => _db.WorkItems
            .AsNoTracking()
            .Include(x => x.Project)
            .ToListAsync(cancellationToken);

    public Task<List<WorkItemRecord>> ListByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
        => _db.WorkItems
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .ToListAsync(cancellationToken);

    public Task AddAsync(WorkItemRecord workItem, CancellationToken cancellationToken = default)
        => _db.WorkItems.AddAsync(workItem, cancellationToken).AsTask();

    public void Remove(WorkItemRecord workItem) => _db.WorkItems.Remove(workItem);
}
