using API.Core.Application.Domain.Dto.Post;
using API.Core.Application.Domain.Interfaces;
using API.Core.Application.Interfaces;
using API.Core.Domain.Entities;
using API.Core.Domain.Interfaces;

namespace API.Core.Application.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IPostCacheService _cache;
    private readonly IRateLimitService _rateLimiter;
    private readonly IPostSearchService _search;

    public PostService(
        IPostRepository postRepository,
        IPostCacheService cache,
        IRateLimitService rateLimiter,
        IPostSearchService search
    )
    {
        _postRepository = postRepository;
        _cache = cache;
        _rateLimiter = rateLimiter;
        _search = search;
    }

    public async Task<PostDto?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        // Cache-aside: check cache first
        Post? cached = await _cache.GetPostAsync(id, cancellationToken);
        if (cached is not null)
            return ToDto(cached);

        // Cache miss — go to MongoDB
        Post? post = await _postRepository.GetByIdAsync(id, cancellationToken);
        if (post is null)
            return null;

        // Populate cache for next read
        await _cache.SetPostAsync(post, cancellationToken);
        return ToDto(post);
    }

    public async Task<PostDto> CreateAsync(
        string blogId,
        CreatePostDto dto,
        CancellationToken cancellationToken = default
    )
    {
        Post post = new()
        {
            BlogId = blogId,
            AuthorId = dto.AuthorId,
            Title = dto.Title,
            Body = dto.Body,
            Tags = dto.Tags,
        };

        Post created = await _postRepository.CreateAsync(post, cancellationToken);

        await _cache.InvalidateBlogPostsAsync(blogId, cancellationToken);
        await _search.IndexPostAsync(created, cancellationToken);

        return ToDto(created);
    }

    public async Task UpdateAsync(
        string id,
        UpdatePostDto dto,
        CancellationToken cancellationToken = default
    )
    {
        Post? post = await _postRepository.GetByIdAsync(id, cancellationToken);
        if (post is null)
            return;

        if (dto.Title is not null)
            post.Title = dto.Title;
        if (dto.Body is not null)
            post.Body = dto.Body;
        if (dto.Tags is not null)
            post.Tags = dto.Tags;
        post.UpdatedAt = DateTime.UtcNow;

        await _postRepository.UpdateAsync(post, cancellationToken);

        await _cache.InvalidatePostAsync(id, cancellationToken);
        await _cache.InvalidateBlogPostsAsync(post.BlogId, cancellationToken);
        await _search.IndexPostAsync(post, cancellationToken);

        return;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        Post? post = await _postRepository.GetByIdAsync(id, cancellationToken);

        await _postRepository.DeleteAsync(id, cancellationToken);

        await _cache.InvalidatePostAsync(id, cancellationToken);
        if (post is not null)
            await _cache.InvalidateBlogPostsAsync(post.BlogId, cancellationToken);

        await _search.RemovePostAsync(id, cancellationToken);
    }

    public async Task AddCommentAsync(
        string postId,
        AddCommentDto dto,
        CancellationToken cancellationToken = default
    )
    {
        // Throws RateLimitExceededException (→ 429) if user is over the limit.
        await _rateLimiter.CheckCommentRateAsync(dto.AuthorId, cancellationToken);

        Comment comment = new() { AuthorId = dto.AuthorId, Body = dto.Body };
        await _postRepository.AddCommentAsync(postId, comment, cancellationToken);
        await _cache.InvalidatePostAsync(postId, cancellationToken);
    }

    private static PostDto ToDto(Post p) =>
        new(p.Id, p.BlogId, p.AuthorId, p.Title, p.Body, p.Tags, p.Comments.Count, p.CreatedAt);
}
