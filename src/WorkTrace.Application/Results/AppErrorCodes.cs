namespace WorkTrace.Application.Results;

public static class AppErrorCodes
{
    public const string ValidationFailed = "application.validation.failed";
    public const string NotFound = "application.not-found";
    public const string ActiveSessionExists = "application.active-session-exists";
    public const string WorkItemNotFound = "application.work-item-not-found";
    public const string WorkSessionNotFound = "application.work-session-not-found";
    public const string NoteNotFound = "application.note-not-found";
    public const string ProjectNotFound = "application.project-not-found";
    public const string InvalidState = "application.invalid-state";
}
