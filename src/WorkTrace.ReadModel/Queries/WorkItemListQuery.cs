using Microsoft.EntityFrameworkCore;
using WorkTrace.Infrastructure.Persistence;
using WorkTrace.ReadModel.Dtos;

namespace WorkTrace.ReadModel.Queries;

public sealed class WorkItemListQuery : IWorkItemListQuery
{
    private readonly WorkTraceDbContext _db;

    public WorkItemListQuery(WorkTraceDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<WorkItemListItemDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var items = await _db.WorkItems
            .AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.WorkSessions)
            .Include(x => x.Notes)
            .Select(item => new WorkItemListItemDto(
                item.Id,
                item.Title,
                item.Kind.ToString(),
                item.Status.ToString(),
                item.ProjectId,
                item.Project != null ? item.Project.Name : null,
                item.IsArchived,
                item.DoneAt,
                item.ArchivedAt,
                item.UpdatedAt,
                item.WorkSessions.Count,
                item.Notes.Count))
            .ToListAsync(cancellationToken);

        return items.OrderByDescending(x => x.UpdatedAt).ToList();
    }
}
