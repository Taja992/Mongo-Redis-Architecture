using API.Core.Domain.Entities;

namespace API.Core.Application.Interfaces;

public interface IPostCacheService
{
    Task<Post?> GetPostAsync(string postId, CancellationToken cancellationToken = default);
    Task SetPostAsync(Post post, CancellationToken cancellationToken = default);
    Task InvalidatePostAsync(string postId, CancellationToken cancellationToken = default);

    Task<List<Post>?> GetBlogPostsAsync(
        string blogId,
        CancellationToken cancellationToken = default
    );
    Task SetBlogPostsAsync(
        string blogId,
        List<Post> posts,
        CancellationToken cancellationToken = default
    );
    Task InvalidateBlogPostsAsync(string blogId, CancellationToken cancellationToken = default);
}
