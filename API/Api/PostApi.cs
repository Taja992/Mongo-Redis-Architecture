using API.Core.Application.Domain.Dto.Post;
using API.Core.Application.Domain.Interfaces;
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

        api.MapGet("/posts/{postId}", GetPost)
            .WithName("Get post by ID.")
            .WithDescription("Fetches a post by its ID.");

        api.MapPut("/posts/{postId}", UpdatePost)
            .WithName("Update a post.")
            .WithDescription("Updates the content of an existing post.");

        api.MapDelete("/posts/{postId}", DeletePost)
            .WithName("Delete a post.")
            .WithDescription("Deletes a post by its ID.");

        api.MapPost("/posts/{postId}/comments", AddComment)
            .WithName("Add a comment.")
            .WithDescription("Adds a comment to a post.");

        api.MapGet("/posts/search", SearchPosts)
            .WithName("Search posts.")
            .WithDescription("Full-text search across post titles and bodies.");

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
        string postId,
        [FromServices] IPostService postService,
        CancellationToken cancellationToken
    )
    {
        PostDto? post = await postService.GetByIdAsync(postId, cancellationToken);
        return post is null ? TypedResults.NotFound() : TypedResults.Ok(post);
    }

    static async Task<NoContent> UpdatePost(
        string postId,
        [FromBody] UpdatePostDto dto,
        [FromServices] IPostService postService,
        CancellationToken cancellationToken
    )
    {
        await postService.UpdateAsync(postId, dto, cancellationToken);
        return TypedResults.NoContent();
    }

    static async Task<NoContent> DeletePost(
        string postId,
        [FromServices] IPostService postService,
        CancellationToken cancellationToken
    )
    {
        await postService.DeleteAsync(postId, cancellationToken);
        return TypedResults.NoContent();
    }

    static async Task<NoContent> AddComment(
        string postId,
        [FromBody] AddCommentDto dto,
        [FromServices] IPostService postService,
        CancellationToken cancellationToken
    )
    {
        await postService.AddCommentAsync(postId, dto, cancellationToken);
        return TypedResults.NoContent();
    }

    static async Task<Ok<List<PostSearchResultDto>>> SearchPosts(
        [FromQuery] string q,
        [FromServices] IPostSearchService searchService,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default
    )
    {
        List<PostSearchResultDto> results = await searchService.SearchAsync(
            q,
            limit,
            cancellationToken
        );
        return TypedResults.Ok(results);
    }
}
