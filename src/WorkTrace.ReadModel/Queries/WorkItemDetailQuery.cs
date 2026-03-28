using Microsoft.EntityFrameworkCore;
using WorkTrace.Infrastructure.Persistence;
using WorkTrace.ReadModel.Dtos;

namespace WorkTrace.ReadModel.Queries;

public sealed class WorkItemDetailQuery : IWorkItemDetailQuery
{
    private readonly WorkTraceDbContext _db;

    public WorkItemDetailQuery(WorkTraceDbContext db)
    {
        _db = db;
    }

    public async Task<WorkItemDetailDto?> GetAsync(Guid workItemId, CancellationToken cancellationToken = default)
    {
        var item = await _db.WorkItems
            .AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.WorkSessions)
            .Include(x => x.Notes)
            .SingleOrDefaultAsync(x => x.Id == workItemId, cancellationToken);

        if (item is null)
        {
            return null;
        }

        return new WorkItemDetailDto(
            item.Id,
            item.Title,
            item.Kind.ToString(),
            item.Status.ToString(),
            item.Description,
            item.ProjectId,
            item.Project?.Name,
            item.IsArchived,
            item.DoneAt,
            item.ArchivedAt,
            item.CreatedAt,
            item.UpdatedAt,
            item.WorkSessions
                .OrderByDescending(x => x.StartedAt)
                .Select(x => new WorkSessionDto(
                    x.Id,
                    x.UserId,
                    x.WorkItemId,
                    x.StartedAt,
                    x.EndedAt,
                    x.EndedAt is null,
                    (x.EndedAt ?? x.UpdatedAt) - x.StartedAt))
                .ToList(),
            item.Notes
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new NoteDto(
                    x.Id,
                    x.Text,
                    x.Type.ToString(),
                    x.CreatedAt,
                    x.EditedAt))
                .ToList());
    }
}
