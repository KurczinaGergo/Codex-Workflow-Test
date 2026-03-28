namespace WorkTrace.Infrastructure.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
