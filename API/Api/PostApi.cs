using API.Core.Application.Commands.Posts;
using API.Core.Application.Domain.Dto.Post;
using API.Core.Application.Domain.Interfaces;
using API.Core.Application.Interfaces;
using API.Core.Application.Queries.Posts;
using MediatR;
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

    static async Task<Results<Created<PostDto>, NotFound>> CreatePost(
        string blogId,
        [FromBody] CreatePostDto dto,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken
    )
    {
        string postId = await mediator.Send(
            new CreatePostCommand(blogId, dto.AuthorId, dto.Title, dto.Body, dto.Tags),
            cancellationToken
        );

        // Query the read model so the response reflects what MongoDB actually stored.
        // This also demonstrates the full CQRS loop: command writes, query reads.
        PostDto? post = await mediator.Send(new GetPostQuery(postId), cancellationToken);
        return post is null
            ? TypedResults.NotFound()
            : TypedResults.Created($"/api/posts/{postId}", post);
    }

    static async Task<Results<Ok<PostDto>, NotFound>> GetPost(
        string postId,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken
    )
    {
        PostDto? post = await mediator.Send(new GetPostQuery(postId), cancellationToken);
        return post is null ? TypedResults.NotFound() : TypedResults.Ok(post);
    }

    static async Task<NoContent> UpdatePost(
        string postId,
        [FromBody] UpdatePostDto dto,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(
            new UpdatePostCommand(postId, dto.Title, dto.Body, dto.Tags),
            cancellationToken
        );
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
