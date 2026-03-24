using MediatR;

namespace API.Core.Application.Commands.Posts;

/// <summary>
/// Writes a new post to PostgreSQL and syncs the MongoDB read model via PostCreatedEvent.
/// Returns the generated post ID so the endpoint can query the read model for the response.
/// </summary>
public record CreatePostCommand(
    string BlogId,
    string AuthorId,
    string Title,
    string Body,
    List<string> Tags
) : IRequest<string>;
