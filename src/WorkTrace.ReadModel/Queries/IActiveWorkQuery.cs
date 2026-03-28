using WorkTrace.ReadModel.Dtos;

namespace WorkTrace.ReadModel.Queries;

public interface IActiveWorkQuery
{
    Task<ActiveWorkDto?> GetCurrentAsync(CancellationToken cancellationToken = default);
}
