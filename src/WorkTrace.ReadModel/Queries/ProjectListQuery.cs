using Microsoft.EntityFrameworkCore;
using WorkTrace.Infrastructure.Persistence;
using WorkTrace.ReadModel.Dtos;

namespace WorkTrace.ReadModel.Queries;

public sealed class ProjectListQuery : IProjectListQuery
{
    private readonly WorkTraceDbContext _db;

    public ProjectListQuery(WorkTraceDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ProjectListItemDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Projects
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(project => new ProjectListItemDto(
                project.Id,
                project.Name,
                project.WorkItems.Count,
                project.WorkItems.Count(item => item.WorkSessions.Any(session => session.EndedAt == null)),
                project.CreatedAt,
                project.UpdatedAt))
            .ToListAsync(cancellationToken);
    }
}
