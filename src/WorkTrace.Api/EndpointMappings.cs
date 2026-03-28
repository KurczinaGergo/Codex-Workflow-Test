namespace WorkTrace.Api;

public static class EndpointMappings
{
    public static IEndpointRouteBuilder MapWorkTraceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api").WithTags("WorkTrace");

        group.MapGet("/health", () => Results.Ok(new { status = "ok" }))
            .WithName("Health")
            .Produces(StatusCodes.Status200OK);

        group.MapGet("/active-work", (IWorkTraceStore store) => Results.Ok(store.GetActiveWork()))
            .WithName("GetActiveWork")
            .Produces<ActiveWorkResponse>(StatusCodes.Status200OK);

        group.MapGet("/work-items", (IWorkTraceStore store) => Results.Ok(store.GetWorkItems()))
            .WithName("GetWorkItems")
            .Produces<IReadOnlyList<WorkItemListItem>>(StatusCodes.Status200OK);

        group.MapGet("/work-items/{id:guid}", (Guid id, IWorkTraceStore store) => ExecuteOk(() => store.GetWorkItem(id)))
            .WithName("GetWorkItem")
            .Produces<WorkItemDetailResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/work-items", (CreateWorkItemRequest request, IWorkTraceStore store) => ExecuteCreated(() => store.CreateWorkItem(request), item => $"/api/work-items/{item.Id}"))
            .WithName("CreateWorkItem")
            .Produces<WorkItemDetailResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPatch("/work-items/{id:guid}/status", (Guid id, UpdateWorkItemStatusRequest request, IWorkTraceStore store) => ExecuteOk(() => store.UpdateWorkItemStatus(id, request)))
            .WithName("UpdateWorkItemStatus")
            .Produces<WorkItemDetailResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/work-items/{id:guid}/archive", (Guid id, IWorkTraceStore store) => ExecuteOk(() => store.ArchiveWorkItem(id)))
            .WithName("ArchiveWorkItem")
            .Produces<WorkItemDetailResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/work-items/{id:guid}/restore", (Guid id, IWorkTraceStore store) => ExecuteOk(() => store.RestoreWorkItem(id)))
            .WithName("RestoreWorkItem")
            .Produces<WorkItemDetailResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/work-items/{id:guid}/sessions", (Guid id, IWorkTraceStore store) => ExecuteCreated(() => store.StartWorkSession(id), session => $"/api/sessions/{session.Id}"))
            .WithName("StartWorkSession")
            .Produces<WorkSessionResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/sessions/{id:guid}/stop", (Guid id, IWorkTraceStore store) => ExecuteOk(() => store.StopWorkSession(id)))
            .WithName("StopWorkSession")
            .Produces<WorkSessionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/work-items/{id:guid}/notes", (Guid id, CreateNoteRequest request, IWorkTraceStore store) => ExecuteCreated(() => store.AddNote(id, request), note => $"/api/notes/{note.Id}"))
            .WithName("AddNote")
            .Produces<NoteResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/projects", (IWorkTraceStore store) => Results.Ok(store.GetProjects()))
            .WithName("GetProjects")
            .Produces<IReadOnlyList<ProjectResponse>>(StatusCodes.Status200OK);

        group.MapPost("/projects", (CreateProjectRequest request, IWorkTraceStore store) => ExecuteCreated(() => store.CreateProject(request), project => $"/api/projects/{project.Id}"))
            .WithName("CreateProject")
            .Produces<ProjectResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPatch("/projects/{id:guid}", (Guid id, RenameProjectRequest request, IWorkTraceStore store) => ExecuteOk(() => store.RenameProject(id, request)))
            .WithName("RenameProject")
            .Produces<ProjectResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/timeline", (IWorkTraceStore store) => Results.Ok(store.GetTimeline()))
            .WithName("GetTimeline")
            .Produces<IReadOnlyList<TimelineEntryResponse>>(StatusCodes.Status200OK);

        return app;
    }

    private static IResult ExecuteOk<T>(Func<T> action)
    {
        try
        {
            return Results.Ok(action());
        }
        catch (ApiProblemException exception)
        {
            return ToProblem(exception);
        }
    }

    private static IResult ExecuteCreated<T>(Func<T> action, Func<T, string> locationFactory)
    {
        try
        {
            var result = action();
            return Results.Created(locationFactory(result), result);
        }
        catch (ApiProblemException exception)
        {
            return ToProblem(exception);
        }
    }

    private static IResult ToProblem(ApiProblemException exception)
    {
        return Results.Problem(
            title: exception.Title,
            detail: exception.Message,
            statusCode: exception.StatusCode);
    }
}
