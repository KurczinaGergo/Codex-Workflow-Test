using WorkTrace.Domain.Shared;

namespace WorkTrace.Application.Abstractions;

public interface ICurrentUser
{
    UserId UserId { get; }
}
