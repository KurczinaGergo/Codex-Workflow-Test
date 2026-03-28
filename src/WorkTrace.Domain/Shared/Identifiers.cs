namespace WorkTrace.Domain.Shared;

public readonly record struct UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
    public static UserId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}

public readonly record struct WorkItemId(Guid Value)
{
    public static WorkItemId New() => new(Guid.NewGuid());
    public static WorkItemId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}

public readonly record struct WorkSessionId(Guid Value)
{
    public static WorkSessionId New() => new(Guid.NewGuid());
    public static WorkSessionId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}

public readonly record struct NoteId(Guid Value)
{
    public static NoteId New() => new(Guid.NewGuid());
    public static NoteId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}

public readonly record struct ProjectId(Guid Value)
{
    public static ProjectId New() => new(Guid.NewGuid());
    public static ProjectId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}
