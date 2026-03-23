using API.Core.Domain.Entities;

namespace API.Core.Domain.Interfaces;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<List<Post>> GetByBlogAsync(string blogId, CancellationToken cancellationToken);
    Task<Post> CreateAsync(Post post, CancellationToken cancellationToken);
    Task UpdateAsync(Post post, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
    Task DeleteByBlogAsync(string blogId, CancellationToken cancellationToken);
    Task AddCommentAsync(string postId, Comment comment, CancellationToken cancellationToken);
}
