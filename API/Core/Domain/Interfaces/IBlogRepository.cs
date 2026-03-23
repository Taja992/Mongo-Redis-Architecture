using API.Core.Domain.Entities;

namespace API.Core.Domain.Interfaces;

public interface IBlogRepository
{
    Task<Blog?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Blog> CreateAsync(Blog blog, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
