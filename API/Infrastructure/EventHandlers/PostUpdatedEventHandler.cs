using API.Core.Application.Events;
using API.Core.Application.Interfaces;
using API.Core.Domain.Interfaces;
using MediatR;

namespace API.Infrastructure.EventHandlers;

/// <summary>
/// Syncs the MongoDB read model when a post is updated in PostgreSQL.
/// Overwrites all fields from the event — no conditional logic needed
/// because the event carries the complete final state.
/// </summary>
public class PostUpdatedEventHandler : INotificationHandler<PostUpdatedEvent>
{
    private readonly IPostRepository _postRepository;
    private readonly IPostCacheService _cache;
    private readonly IPostSearchService _search;

    public PostUpdatedEventHandler(
        IPostRepository postRepository,
        IPostCacheService cache,
        IPostSearchService search
    )
    {
        _postRepository = postRepository;
        _cache = cache;
        _search = search;
    }

    public async Task Handle(PostUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(notification.PostId, cancellationToken);
        if (post is null)
            return;

        post.Title = notification.Title;
        post.Body = notification.Body;
        post.Tags = notification.Tags;
        post.UpdatedAt = notification.UpdatedAt;

        await _postRepository.UpdateAsync(post, cancellationToken);
        await _cache.InvalidatePostAsync(notification.PostId, cancellationToken);
        await _cache.InvalidateBlogPostsAsync(notification.BlogId, cancellationToken);
        await _search.IndexPostAsync(post, cancellationToken);
    }
}
