using API.Core.Application.Domain.Dto.Post;

namespace API.Core.Application.Domain.Interfaces;

public interface IPostService
{
    Task<PostDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<PostDto> CreateAsync(
        string blogId,
        CreatePostDto dto,
        CancellationToken cancellationToken = default
    );
    Task UpdateAsync(string id, UpdatePostDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task AddCommentAsync(
        string postId,
        AddCommentDto dto,
        CancellationToken cancellationToken = default
    );
}
