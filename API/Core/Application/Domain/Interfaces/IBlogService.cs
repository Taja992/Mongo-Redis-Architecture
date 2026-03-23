using API.Core.Application.Domain.Dto.Blog;

namespace API.Core.Application.Interfaces;

public interface IBlogService
{
    Task<BlogDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<BlogDto> CreateAsync(CreateBlogDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
