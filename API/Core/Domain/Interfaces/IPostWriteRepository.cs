using API.Core.Domain.WriteModels;

namespace API.Core.Domain.Interfaces;

/// <summary>
/// Write-side repository for posts. Targets PostgreSQL.
/// Only used by command handlers — queries always go through IPostRepository (MongoDB).
/// </summary>
public interface IPostWriteRepository
{
    Task<PostWriteModel> CreateAsync(PostWriteModel post, CancellationToken cancellationToken);
    Task<PostWriteModel?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task UpdateAsync(PostWriteModel post, CancellationToken cancellationToken);
}
