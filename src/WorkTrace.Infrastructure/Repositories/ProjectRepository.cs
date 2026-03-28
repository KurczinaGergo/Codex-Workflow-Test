using Microsoft.EntityFrameworkCore;
using WorkTrace.Infrastructure.Persistence;
using WorkTrace.Infrastructure.Persistence.Entities;

namespace WorkTrace.Infrastructure.Repositories;

public sealed class ProjectRepository
{
    private readonly WorkTraceDbContext _db;

    public ProjectRepository(WorkTraceDbContext db)
    {
        _db = db;
    }

    public Task<ProjectRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Projects.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<List<ProjectRecord>> ListAsync(CancellationToken cancellationToken = default)
        => _db.Projects.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public Task AddAsync(ProjectRecord project, CancellationToken cancellationToken = default)
        => _db.Projects.AddAsync(project, cancellationToken).AsTask();

    public void Remove(ProjectRecord project) => _db.Projects.Remove(project);
}
