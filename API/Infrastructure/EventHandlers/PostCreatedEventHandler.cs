using API.Core.Application.Events;
using API.Core.Application.Interfaces;
using API.Core.Domain.Entities;
using API.Core.Domain.Interfaces;
using MediatR;

namespace API.Infrastructure.EventHandlers;

/// <summary>
/// Syncs the MongoDB read model when a post is created in PostgreSQL.
/// Uses the same ID generated in the command handler so both stores share the key.
/// </summary>
public class PostCreatedEventHandler : INotificationHandler<PostCreatedEvent>
{
    private readonly IPostRepository _postRepository;
    private readonly IPostCacheService _cache;
    private readonly IPostSearchService _search;

    public PostCreatedEventHandler(
        IPostRepository postRepository,
        IPostCacheService cache,
        IPostSearchService search
    )
    {
        _postRepository = postRepository;
        _cache = cache;
        _search = search;
    }

    public async Task Handle(PostCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Build the MongoDB document with the pre-set ID from the command handler.
        // InsertOneAsync respects a pre-set [BsonId] value — both stores share the same key.
        Post post = new()
        {
            Id = notification.PostId,
            BlogId = notification.BlogId,
            AuthorId = notification.AuthorId,
            Title = notification.Title,
            Body = notification.Body,
            Tags = notification.Tags,
            CreatedAt = notification.CreatedAt,
            UpdatedAt = notification.CreatedAt,
        };

        await _postRepository.CreateAsync(post, cancellationToken);
        await _cache.InvalidateBlogPostsAsync(notification.BlogId, cancellationToken);
        await _search.IndexPostAsync(post, cancellationToken);
    }
}
