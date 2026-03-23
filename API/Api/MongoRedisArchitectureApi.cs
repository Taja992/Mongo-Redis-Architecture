namespace API.Api;

public static class MongoRedisArchitecturesApi
{
    public static RouteGroupBuilder MapMongoRedisArchitecturesApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/mongoredisarchitecture")
            .AllowAnonymous()
            .WithTags("mongoredisarchitecture");

        api.MapGet("/", GetAll)
            .WithName("Get all mongoredisarchitecture.")
            .WithDescription("Fetches all mongoredisarchitecture.");

        api.MapGet("/{mongoredisarchitectureId:long}", GetById)
            .WithName("Get mongoredisarchitecture by ID.")
            .WithDescription("Fetches an mongoredisarchitecture by its ID.");

        api.MapPost("/", CreateMongoRedisArchitecture)
            .WithName("Creates an mongoredisarchitecture.")
            .WithDescription("Create a new mongoredisarchitecture.");

        api.MapPut("/{mongoredisarchitectureId:long}", UpdateMongoRedisArchitecture)
            .WithName("Update an mongoredisarchitecture.")
            .WithDescription("Updates an existing mongoredisarchitecture.");

        api.MapDelete("/{mongoredisarchitectureId:long}", DeleteMongoRedisArchitecture)
            .WithName("Delete mongoredisarchitecture.")
            .WithDescription("Delete an mongoredisarchitecture.");

        return api;
    }

    private static async Task<IResult> GetAll()
    {
        await Task.CompletedTask;
        return Results.Ok(new[] { "MongoRedisArchitecture1", "MongoRedisArchitecture2" }); // Example data
    }

    private static async Task<IResult> GetById(long mongoredisarchitectureId)
    {
        await Task.CompletedTask;
        return Results.Ok($"MongoRedisArchitecture {mongoredisarchitectureId}");
    }

    private static async Task<IResult> CreateMongoRedisArchitecture()
    {
        await Task.CompletedTask;
        return Results.Created(
            "/mongoredisarchitecture/1",
            new { Id = 1, Name = "New MongoRedisArchitecture" }
        );
    }

    private static async Task<IResult> UpdateMongoRedisArchitecture(long mongoredisarchitectureId)
    {
        await Task.CompletedTask;
        return Results.Ok($"Updated mongoredisarchitecture {mongoredisarchitectureId}");
    }

    private static async Task<IResult> DeleteMongoRedisArchitecture(long mongoredisarchitectureId)
    {
        await Task.CompletedTask;
        return Results.NoContent();
    }
}
