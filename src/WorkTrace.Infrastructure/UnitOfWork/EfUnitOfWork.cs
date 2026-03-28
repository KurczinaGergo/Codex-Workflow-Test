using WorkTrace.Infrastructure.Abstractions;
using WorkTrace.Infrastructure.Persistence;

namespace WorkTrace.Infrastructure.UnitOfWork;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly WorkTraceDbContext _db;

    public EfUnitOfWork(WorkTraceDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);
}
