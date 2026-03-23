using API.Core.Domain.Entities;
using API.Core.Domain.Interfaces;
using MongoDB.Driver;

namespace API.Infrastructure.Repositories;

public class MongoPostRepository : IPostRepository
{
    private readonly IMongoCollection<Post> _posts;

    public MongoPostRepository(IMongoDatabase db) => _posts = db.GetCollection<Post>("posts");

    public async Task<Post?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await _posts.Find(p => p.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<List<Post>> GetByBlogAsync(
        string blogId,
        CancellationToken cancellationToken
    ) => await _posts.Find(p => p.BlogId == blogId).ToListAsync(cancellationToken);

    public async Task<Post> CreateAsync(Post post, CancellationToken cancellationToken)
    {
        await _posts.InsertOneAsync(post, cancellationToken: cancellationToken);
        return post;
    }

    public async Task UpdateAsync(Post post, CancellationToken cancellationToken) =>
        await _posts.ReplaceOneAsync(
            p => p.Id == post.Id,
            post,
            cancellationToken: cancellationToken
        );

    public async Task DeleteAsync(string id, CancellationToken cancellationToken) =>
        await _posts.DeleteOneAsync(p => p.Id == id, cancellationToken);

    public async Task DeleteByBlogAsync(string blogId, CancellationToken cancellationToken) =>
        await _posts.DeleteManyAsync(p => p.BlogId == blogId, cancellationToken);

    public async Task AddCommentAsync(
        string postId,
        Comment comment,
        CancellationToken cancellationToken
    ) =>
        await _posts.UpdateOneAsync(
            p => p.Id == postId,
            Builders<Post>.Update.Push(p => p.Comments, comment),
            cancellationToken: cancellationToken
        );
}
