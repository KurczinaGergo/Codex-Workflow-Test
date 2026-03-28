using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WorkTrace.Infrastructure.Abstractions;
using WorkTrace.Infrastructure.Clock;
using WorkTrace.Infrastructure.CurrentUser;
using WorkTrace.Infrastructure.Persistence;
using WorkTrace.Infrastructure.Repositories;
using WorkTrace.Infrastructure.UnitOfWork;

namespace WorkTrace.Infrastructure.DependencyInjection;

public static class WorkTraceInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddWorkTraceInfrastructure(
        this IServiceCollection services,
        string? connectionString = null,
        string? databasePath = null,
        string fixedUserId = "worktrace-user")
    {
        services.AddDbContext<WorkTraceDbContext>(options =>
        {
            options.UseSqlite(BuildConnectionString(connectionString, databasePath));
        });

        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<ICurrentUser>(_ => new FixedCurrentUser(fixedUserId));
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<ProjectRepository>();
        services.AddScoped<WorkItemRepository>();
        services.AddScoped<WorkSessionRepository>();
        services.AddScoped<NoteRepository>();

        return services;
    }

    private static string BuildConnectionString(string? connectionString, string? databasePath)
    {
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return connectionString;
        }

        var path = databasePath;
        if (string.IsNullOrWhiteSpace(path))
        {
            path = Path.Combine(AppContext.BaseDirectory, "data", "worktrace.db");
        }

        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return $"Data Source={path}";
    }
}
