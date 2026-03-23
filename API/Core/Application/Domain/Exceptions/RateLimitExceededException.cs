namespace API.Core.Domain.Exceptions;

public class RateLimitExceededException : AppException
{
    public RateLimitExceededException()
        : base(
            "You have exceeded the comment rate limit. Please try again later.",
            "RATE_LIMIT_EXCEEDED",
            429
        ) { }

    public RateLimitExceededException(int limit, int windowSeconds)
        : base(
            $"You may post at most {limit} comments per {windowSeconds / 60} minutes.",
            "RATE_LIMIT_EXCEEDED",
            429
        ) { }
}
