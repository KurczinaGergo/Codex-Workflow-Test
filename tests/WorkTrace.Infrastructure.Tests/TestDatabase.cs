using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WorkTrace.Infrastructure.Persistence;

namespace WorkTrace.Infrastructure.Tests;

internal sealed class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public TestDatabase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        using var command = _connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys = ON;";
        command.ExecuteNonQuery();
    }

    public WorkTraceDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<WorkTraceDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        var context = new WorkTraceDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public void Dispose() => _connection.Dispose();
}
