using MediatR;

namespace API.Core.Application.Commands.Posts;

/// <summary>
/// Updates an existing post in PostgreSQL and syncs MongoDB via PostUpdatedEvent.
/// Null fields are ignored — only provided fields are applied (PATCH semantics).
/// </summary>
public record UpdatePostCommand(string PostId, string? Title, string? Body, List<string>? Tags)
    : IRequest;
