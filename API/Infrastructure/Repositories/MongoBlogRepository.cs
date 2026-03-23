using API.Core.Domain.Entities;
using API.Core.Domain.Interfaces;
using MongoDB.Driver;

namespace API.Infrastructure.Repositories;

public class MongoBlogRepository : IBlogRepository
{
    private readonly IMongoCollection<Blog> _blogs;

    public MongoBlogRepository(IMongoDatabase db) => _blogs = db.GetCollection<Blog>("blogs");

    public async Task<Blog?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await _blogs.Find(b => b.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<Blog> CreateAsync(Blog blog, CancellationToken cancellationToken)
    {
        await _blogs.InsertOneAsync(blog, cancellationToken: cancellationToken);
        return blog;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken) =>
        await _blogs.DeleteOneAsync(b => b.Id == id, cancellationToken);
}
