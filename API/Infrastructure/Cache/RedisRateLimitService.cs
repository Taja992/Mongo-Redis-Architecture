using API.Core.Application.Domain.Interfaces;
using API.Core.Domain.Exceptions;
using StackExchange.Redis;

namespace API.Infrastructure.Cache;

public class RedisRateLimitService : IRateLimitService
{
    private readonly IDatabase _redis;
    private readonly int _limit;
    private readonly int _windowSeconds;

    public RedisRateLimitService(IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _redis = redis.GetDatabase();
        _limit = configuration.GetValue<int>("RateLimit:CommentMaxPerWindow");
        _windowSeconds = configuration.GetValue<int>("RateLimit:CommentWindowSeconds");
    }

    public async Task CheckCommentRateAsync(
        string authorId,
        CancellationToken cancellationToken = default
    )
    {
        RedisKey key = CommentRateKey(authorId);

        // Increment returns the value *after* incrementing — atomic, no separate read needed.
        long count = await _redis.StringIncrementAsync(key);

        if (count == 1)
        {
            // First comment in this window — set the expiry to start the clock.
            await _redis.KeyExpireAsync(key, TimeSpan.FromSeconds(_windowSeconds));
        }

        if (count > _limit)
        {
            throw new RateLimitExceededException(_limit, _windowSeconds);
        }
    }

    private static RedisKey CommentRateKey(string authorId) => $"rate:comment:{authorId}";
}
