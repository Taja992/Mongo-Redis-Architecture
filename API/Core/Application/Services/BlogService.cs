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

    public BlogService(IBlogRepository blogRepository, IPostRepository postRepository)
    {
        _blogRepository = blogRepository;
        _postRepository = postRepository;
    }

    public async Task<BlogDto?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        Blog? blog = await _blogRepository.GetByIdAsync(id, cancellationToken);
        if (blog is null)
            return null;

        List<Post> posts = await _postRepository.GetByBlogAsync(id, cancellationToken);

        return new BlogDto(
            blog.Id,
            blog.UserId,
            blog.Title,
            blog.Description,
            blog.Tags,
            [
                .. posts.Select(p => new PostDto(
                    p.Id,
                    p.BlogId,
                    p.AuthorId,
                    p.Title,
                    p.Body,
                    p.Tags,
                    p.Comments.Count,
                    p.CreatedAt
                )),
            ],
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
        // Delete posts first, then the blog
        // If DeleteByBlogAsync succeeds but DeleteAsync fails, posts are gone but blog remains.
        // TODO: wrap in a transaction if data consistency is required
        await _postRepository.DeleteByBlogAsync(id, cancellationToken);
        await _blogRepository.DeleteAsync(id, cancellationToken);
    }
}
