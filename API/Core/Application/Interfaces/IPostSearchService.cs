using API.Core.Application.Domain.Dto.Post;
using API.Core.Domain.Entities;

namespace API.Core.Application.Interfaces;

public interface IPostSearchService
{
    Task EnsureIndexAsync(CancellationToken cancellationToken = default);
    Task IndexPostAsync(Post post, CancellationToken cancellationToken = default);
    Task RemovePostAsync(string postId, CancellationToken cancellationToken = default);
    Task<List<PostSearchResultDto>> SearchAsync(
        string query,
        int limit = 10,
        CancellationToken cancellationToken = default
    );
}
