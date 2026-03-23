namespace API.Core.Application.Domain.Interfaces;

public interface IRateLimitService
{
    /// <summary>
    /// Checks whether the user is within the comment rate limit.
    /// Throws <see cref="API.Core.Domain.Exceptions.RateLimitExceededException"/> if exceeded.
    /// </summary>
    Task CheckCommentRateAsync(string authorId, CancellationToken cancellationToken = default);
}
