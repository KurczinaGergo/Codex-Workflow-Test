using Microsoft.Extensions.DependencyInjection;
using WorkTrace.ReadModel.Queries;

namespace WorkTrace.ReadModel.DependencyInjection;

public static class WorkTraceReadModelServiceCollectionExtensions
{
    public static IServiceCollection AddWorkTraceReadModel(this IServiceCollection services)
    {
        services.AddScoped<IActiveWorkQuery, ActiveWorkQuery>();
        services.AddScoped<IWorkItemListQuery, WorkItemListQuery>();
        services.AddScoped<IWorkItemDetailQuery, WorkItemDetailQuery>();
        services.AddScoped<ITimelineQuery, TimelineQuery>();
        services.AddScoped<IProjectListQuery, ProjectListQuery>();

        return services;
    }
}
