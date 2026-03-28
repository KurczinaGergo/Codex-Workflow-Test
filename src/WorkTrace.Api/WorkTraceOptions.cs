namespace WorkTrace.Api;

public sealed class WorkTraceOptions
{
    public const string SectionName = "WorkTrace";

    public Guid CurrentUserId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public string CurrentUserName { get; set; } = "Local MVP User";
}

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
