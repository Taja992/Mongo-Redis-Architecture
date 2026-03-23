using API.Core.Application.Domain.Dto.Blog;
using API.Core.Application.Domain.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Api;

public static class BlogApi
{
    public static RouteGroupBuilder MapBlogApi(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder api = app.MapGroup("/api/blogs").AllowAnonymous().WithTags("Blogs");

        api.MapPost("/", CreateBlog)
            .WithName("Create a blog.")
            .WithDescription("Creates a new blog for a user.");

        api.MapGet("/{id}", GetBlog)
            .WithName("Get blog by ID.")
            .WithDescription("Fetches a blog and its posts by ID.");

        api.MapDelete("/{id}", DeleteBlog)
            .WithName("Delete a blog.")
            .WithDescription("Deletes a blog and all of its posts.");

        return api;
    }

    static async Task<Results<Created<BlogDto>, ProblemHttpResult>> CreateBlog(
        [FromBody] CreateBlogDto dto,
        [FromServices] IBlogService blogService,
        CancellationToken cancellationToken
    )
    {
        BlogDto blog = await blogService.CreateAsync(dto, cancellationToken);
        return TypedResults.Created($"/api/blogs/{blog.Id}", blog);
    }

    static async Task<Results<Ok<BlogDto>, NotFound>> GetBlog(
        string id,
        [FromServices] IBlogService blogService,
        CancellationToken cancellationToken
    )
    {
        BlogDto? blog = await blogService.GetByIdAsync(id, cancellationToken);
        return blog is null ? TypedResults.NotFound() : TypedResults.Ok(blog);
    }

    static async Task<NoContent> DeleteBlog(
        string id,
        [FromServices] IBlogService blogService,
        CancellationToken cancellationToken
    )
    {
        await blogService.DeleteAsync(id, cancellationToken);
        return TypedResults.NoContent();
    }
}
