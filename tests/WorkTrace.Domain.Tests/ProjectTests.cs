using WorkTrace.Domain.Shared;
using WorkTrace.Domain.Projects;

namespace WorkTrace.Domain.Tests;

public class ProjectTests
{
    [Fact]
    public void Create_RejectsBlankName()
    {
        var ex = Assert.Throws<DomainException>(() => Project.Create(" ", DateTimeOffset.UtcNow));

        Assert.Equal(DomainErrorCodes.RequiredValue, ex.Code);
    }

    [Fact]
    public void Rename_ChangesUpdatedAt_WhenNameChanges()
    {
        var createdAt = new DateTimeOffset(2026, 03, 28, 12, 0, 0, TimeSpan.Zero);
        var project = Project.Create("Alpha", createdAt);
        var renamedAt = createdAt.AddHours(1);

        project.Rename("Beta", renamedAt);

        Assert.Equal("Beta", project.Name);
        Assert.Equal(renamedAt, project.UpdatedAt);
    }
}
