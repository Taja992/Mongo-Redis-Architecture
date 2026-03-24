using API.Core.Domain.Interfaces;
using API.Core.Domain.WriteModels;
using Microsoft.EntityFrameworkCore;

namespace API.Infrastructure.SqlRepositories;

public class SqlPostWriteRepository : IPostWriteRepository
{
    private readonly AppDbContext _db;

    public SqlPostWriteRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PostWriteModel> CreateAsync(
        PostWriteModel post,
        CancellationToken cancellationToken
    )
    {
        _db.Posts.Add(post);
        await _db.SaveChangesAsync(cancellationToken);
        return post;
    }

    public async Task<PostWriteModel?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken
    ) => await _db.Posts.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task UpdateAsync(PostWriteModel post, CancellationToken cancellationToken)
    {
        _db.Posts.Update(post);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
