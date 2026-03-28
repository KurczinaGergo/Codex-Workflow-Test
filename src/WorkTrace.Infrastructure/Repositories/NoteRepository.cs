using Microsoft.EntityFrameworkCore;
using WorkTrace.Infrastructure.Persistence;
using WorkTrace.Infrastructure.Persistence.Entities;

namespace WorkTrace.Infrastructure.Repositories;

public sealed class NoteRepository
{
    private readonly WorkTraceDbContext _db;

    public NoteRepository(WorkTraceDbContext db)
    {
        _db = db;
    }

    public Task<NoteRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Notes
            .Include(x => x.WorkItem)
            .ThenInclude(x => x!.Project)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<List<NoteRecord>> ListByWorkItemIdAsync(Guid workItemId, CancellationToken cancellationToken = default)
        => _db.Notes
            .AsNoTracking()
            .Where(x => x.WorkItemId == workItemId)
            .ToListAsync(cancellationToken);

    public Task AddAsync(NoteRecord note, CancellationToken cancellationToken = default)
        => _db.Notes.AddAsync(note, cancellationToken).AsTask();

    public void Remove(NoteRecord note) => _db.Notes.Remove(note);
}
