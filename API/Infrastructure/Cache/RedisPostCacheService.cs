using System.Text.Json;
using API.Core.Application.Interfaces;
using API.Core.Domain.Entities;
using StackExchange.Redis;

namespace API.Infrastructure.Cache;

public class RedisPostCacheService : IPostCacheService
{
    private readonly IDatabase _redis;
    private readonly TimeSpan? _ttl;

    public RedisPostCacheService(IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _redis = redis.GetDatabase();

        int ttlSeconds = configuration.GetValue<int>("Redis:PostTtlSeconds");
        _ttl = ttlSeconds > 0 ? TimeSpan.FromSeconds(ttlSeconds) : null;
    }

    public async Task<Post?> GetPostAsync(
        string postId,
        CancellationToken cancellationToken = default
    )
    {
        RedisValue value = await _redis.StringGetAsync(PostKey(postId));
        return value.HasValue ? JsonSerializer.Deserialize<Post>((string)value!) : null;
    }

    public async Task SetPostAsync(Post post, CancellationToken cancellationToken = default)
    {
        string serialized = JsonSerializer.Serialize(post);
        if (_ttl is null)
            await _redis.StringSetAsync(PostKey(post.Id), serialized);
        else
            await _redis.StringSetAsync(PostKey(post.Id), serialized, _ttl.Value);
    }

    public async Task InvalidatePostAsync(
        string postId,
        CancellationToken cancellationToken = default
    )
    {
        await _redis.KeyDeleteAsync(PostKey(postId));
    }

    public async Task<List<Post>?> GetBlogPostsAsync(
        string blogId,
        CancellationToken cancellationToken = default
    )
    {
        RedisValue value = await _redis.StringGetAsync(BlogPostsKey(blogId));
        return value.HasValue ? JsonSerializer.Deserialize<List<Post>>((string)value!) : null;
    }

    public async Task SetBlogPostsAsync(
        string blogId,
        List<Post> posts,
        CancellationToken cancellationToken = default
    )
    {
        string serialized = JsonSerializer.Serialize(posts);
        if (_ttl is null)
            await _redis.StringSetAsync(BlogPostsKey(blogId), serialized);
        else
            await _redis.StringSetAsync(BlogPostsKey(blogId), serialized, _ttl.Value);
    }

    public async Task InvalidateBlogPostsAsync(
        string blogId,
        CancellationToken cancellationToken = default
    )
    {
        await _redis.KeyDeleteAsync(BlogPostsKey(blogId));
    }

    private static string PostKey(string postId) => $"post:{postId}";

    private static string BlogPostsKey(string blogId) => $"blog:{blogId}:posts";
}
