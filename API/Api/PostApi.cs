using API.Core.Application.Domain.Dto.Post;
using API.Core.Application.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Api;

public static class PostApi
{
    public static RouteGroupBuilder MapPostApi(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder api = app.MapGroup("/api").AllowAnonymous().WithTags("Posts");

        api.MapPost("/blogs/{blogId}/posts", CreatePost)
            .WithName("Create a post.")
            .WithDescription("Creates a new post inside a blog.");

        api.MapGet("/posts/{id}", GetPost)
            .WithName("Get post by ID.")
            .WithDescription("Fetches a post by its ID.");

        api.MapPut("/posts/{id}", UpdatePost)
            .WithName("Update a post.")
            .WithDescription("Updates the content of an existing post.");

        api.MapDelete("/posts/{id}", DeletePost)
            .WithName("Delete a post.")
            .WithDescription("Deletes a post by its ID.");

        api.MapPost("/posts/{id}/comments", AddComment)
            .WithName("Add a comment.")
            .WithDescription("Adds a comment to a post.");

        return api;
    }

    static async Task<Results<Created<PostDto>, ProblemHttpResult>> CreatePost(
        string blogId,
        [FromBody] CreatePostDto dto,
        [FromServices] IPostService postService,
        CancellationToken cancellationToken
    )
    {
        PostDto post = await postService.CreateAsync(blogId, dto, cancellationToken);
        return TypedResults.Created($"/api/posts/{post.Id}", post);
    }

    static async Task<Results<Ok<PostDto>, NotFound>> GetPost(
        string id,
        [FromServices] IPostService postService,
        CancellationToken cancellationToken
    )
    {
        PostDto? post = await postService.GetByIdAsync(id, cancellationToken);
        return post is null ? TypedResults.NotFound() : TypedResults.Ok(post);
    }

    static async Task<NoContent> UpdatePost(
        string id,
        [FromBody] UpdatePostDto dto,
        [FromServices] IPostService postService,
        CancellationToken cancellationToken
    )
    {
        await postService.UpdateAsync(id, dto, cancellationToken);
        return TypedResults.NoContent();
    }

    static async Task<NoContent> DeletePost(
        string id,
        [FromServices] IPostService postService,
        CancellationToken cancellationToken
    )
    {
        await postService.DeleteAsync(id, cancellationToken);
        return TypedResults.NoContent();
    }

    static async Task<NoContent> AddComment(
        string id,
        [FromBody] AddCommentDto dto,
        [FromServices] IPostService postService,
        CancellationToken cancellationToken
    )
    {
        await postService.AddCommentAsync(id, dto, cancellationToken);
        return TypedResults.NoContent();
    }
}
