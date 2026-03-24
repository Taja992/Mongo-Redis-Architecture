using API.Core.Domain;
using MediatR;

namespace API.Core.Application.Events;

/// <summary>
/// Published after UpdatePostCommand writes to PostgreSQL.
/// Carries the full resolved state (not just the changed fields) so the
/// event handler can overwrite without needing to know what changed.
/// </summary>
public record PostUpdatedEvent(
    string PostId,
    string BlogId,
    string Title,
    string Body,
    List<string> Tags,
    DateTime UpdatedAt
) : IDomainEvent, INotification;
