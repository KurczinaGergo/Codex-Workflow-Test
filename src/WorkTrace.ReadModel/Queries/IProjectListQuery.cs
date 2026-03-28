using WorkTrace.ReadModel.Dtos;

namespace WorkTrace.ReadModel.Queries;

public interface IProjectListQuery
{
    Task<IReadOnlyList<ProjectListItemDto>> GetAsync(CancellationToken cancellationToken = default);
}
