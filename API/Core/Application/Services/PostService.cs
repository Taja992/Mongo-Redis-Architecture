using API.Core.Application.Domain.Dto.Post;
using API.Core.Application.Interfaces;
using API.Core.Domain.Entities;
using API.Core.Domain.Interfaces;

namespace API.Core.Application.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<PostDto?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        Post? post = await _postRepository.GetByIdAsync(id, cancellationToken);
        if (post is null)
            return null;

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
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        await _postRepository.DeleteAsync(id, cancellationToken);

    public async Task AddCommentAsync(
        string postId,
        AddCommentDto dto,
        CancellationToken cancellationToken = default
    )
    {
        Comment comment = new() { AuthorId = dto.AuthorId, Body = dto.Body };

        await _postRepository.AddCommentAsync(postId, comment, cancellationToken);
    }

    private static PostDto ToDto(Post p) =>
        new(p.Id, p.BlogId, p.AuthorId, p.Title, p.Body, p.Tags, p.Comments.Count, p.CreatedAt);
}
