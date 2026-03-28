using WorkTrace.Domain.Notes;
using WorkTrace.Domain.Shared;
using WorkTrace.Domain.WorkItems;

namespace WorkTrace.Domain.Tests;

public class NoteTests
{
    [Fact]
    public void Create_RejectsBlankText()
    {
        var ex = Assert.Throws<DomainException>(() => Note.Create(WorkItemId.New(), " ", NoteType.Human, DateTimeOffset.UtcNow));

        Assert.Equal(DomainErrorCodes.RequiredValue, ex.Code);
    }

    [Fact]
    public void EditText_SetsEditedAtOnlyWhenTextChanges()
    {
        var createdAt = new DateTimeOffset(2026, 03, 28, 12, 0, 0, TimeSpan.Zero);
        var note = Note.Create(WorkItemId.New(), "Initial", NoteType.Human, createdAt);
        var editedAt = createdAt.AddMinutes(10);

        note.EditText("Updated", editedAt);
        note.EditText("Updated", editedAt.AddMinutes(5));

        Assert.Equal("Updated", note.Text);
        Assert.Equal(editedAt, note.EditedAt);
    }
}
