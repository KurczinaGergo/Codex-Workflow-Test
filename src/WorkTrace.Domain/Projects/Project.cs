using WorkTrace.Domain.Shared;

namespace WorkTrace.Domain.Projects;

public sealed class Project
{
    private Project(ProjectId id, string name, DateTimeOffset createdAt, DateTimeOffset updatedAt)
    {
        Id = id;
        Name = name;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public ProjectId Id { get; }

    public string Name { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Project Create(string name, DateTimeOffset createdAt)
    {
        return new Project(
            ProjectId.New(),
            Guard.AgainstNullOrWhiteSpace(name, nameof(name), "Project name is required."),
            createdAt,
            createdAt);
    }

    public void Rename(string name, DateTimeOffset updatedAt)
    {
        var normalized = Guard.AgainstNullOrWhiteSpace(name, nameof(name), "Project name is required.");

        if (Name != normalized)
        {
            Name = normalized;
            UpdatedAt = updatedAt;
        }
    }
}
