using API.Core.Application.Domain.Dto.Blog;
using API.Core.Application.Domain.Dto.Post;
using API.Core.Application.Interfaces;
using API.Core.Domain.Entities;
using API.Core.Domain.Interfaces;

namespace API.Core.Application.Services;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly IPostRepository _postRepository;
    private readonly IPostCacheService _cache;

    public BlogService(
        IBlogRepository blogRepository,
        IPostRepository postRepository,
        IPostCacheService cache
    )
    {
        _blogRepository = blogRepository;
        _postRepository = postRepository;
        _cache = cache;
    }

    public async Task<BlogDto?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        Blog? blog = await _blogRepository.GetByIdAsync(id, cancellationToken);
        if (blog is null)
            return null;

        // Cache-aside for the post list
        List<Post>? cachedPosts = await _cache.GetBlogPostsAsync(id, cancellationToken);

        List<Post> posts;
        if (cachedPosts is not null)
        {
            posts = cachedPosts;
        }
        else
        {
            posts = await _postRepository.GetByBlogAsync(id, cancellationToken);
            await _cache.SetBlogPostsAsync(id, posts, cancellationToken);
        }

        return new BlogDto(
            blog.Id,
            blog.UserId,
            blog.Title,
            blog.Description,
            blog.Tags,
            posts
                .Select(p => new PostDto(
                    p.Id,
                    p.BlogId,
                    p.AuthorId,
                    p.Title,
                    p.Body,
                    p.Tags,
                    p.Comments.Count,
                    p.CreatedAt
                ))
                .ToList(),
            blog.CreatedAt
        );
    }

    public async Task<BlogDto> CreateAsync(
        CreateBlogDto dto,
        CancellationToken cancellationToken = default
    )
    {
        Blog blog = new()
        {
            UserId = dto.UserId,
            Title = dto.Title,
            Description = dto.Description,
            Tags = dto.Tags,
        };

        Blog created = await _blogRepository.CreateAsync(blog, cancellationToken);
        return new BlogDto(
            created.Id,
            created.UserId,
            created.Title,
            created.Description,
            created.Tags,
            [],
            created.CreatedAt
        );
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _postRepository.DeleteByBlogAsync(id, cancellationToken);
        await _blogRepository.DeleteAsync(id, cancellationToken);

        // All posts for this blog are gone — wipe the list cache
        await _cache.InvalidateBlogPostsAsync(id, cancellationToken);
    }
}
