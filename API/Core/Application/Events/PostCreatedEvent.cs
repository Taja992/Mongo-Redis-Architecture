using API.Core.Domain;
using MediatR;

namespace API.Core.Application.Events;

/// <summary>
/// Published after CreatePostCommand writes to PostgreSQL.
/// Consumed by PostCreatedEventHandler to sync the MongoDB read model.
/// </summary>
public record PostCreatedEvent(
    string PostId,
    string BlogId,
    string AuthorId,
    string Title,
    string Body,
    List<string> Tags,
    DateTime CreatedAt
) : IDomainEvent, INotification;
