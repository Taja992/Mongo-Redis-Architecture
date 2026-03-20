namespace API.Api;

public static class TemplatesApi
{
    public static RouteGroupBuilder MapTemplatesApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/template").AllowAnonymous().WithTags("template");

        api.MapGet("/", GetAll)
            .WithName("Get all template.")
            .WithDescription("Fetches all template.");

        api.MapGet("/{templateId:long}", GetById)
            .WithName("Get template by ID.")
            .WithDescription("Fetches an template by its ID.");

        api.MapPost("/", CreateTemplate)
            .WithName("Creates an template.")
            .WithDescription("Create a new template.");

        api.MapPut("/{templateId:long}", UpdateTemplate)
            .WithName("Update an template.")
            .WithDescription("Updates an existing template.");

        api.MapDelete("/{templateId:long}", DeleteTemplate)
            .WithName("Delete template.")
            .WithDescription("Delete an template.");

        return api;
    }

    private static async Task<IResult> GetAll()
    {
        await Task.CompletedTask;
        return Results.Ok(new[] { "Template1", "Template2" }); // Example data
    }

    private static async Task<IResult> GetById(long templateId)
    {
        await Task.CompletedTask;
        return Results.Ok($"Template {templateId}");
    }

    private static async Task<IResult> CreateTemplate()
    {
        await Task.CompletedTask;
        return Results.Created("/template/1", new { Id = 1, Name = "New Template" });
    }

    private static async Task<IResult> UpdateTemplate(long templateId)
    {
        await Task.CompletedTask;
        return Results.Ok($"Updated template {templateId}");
    }

    private static async Task<IResult> DeleteTemplate(long templateId)
    {
        await Task.CompletedTask;
        return Results.NoContent();
    }
}
