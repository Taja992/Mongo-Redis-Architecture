using API.Core.Application.Domain.Dto.Post;
using MediatR;

namespace API.Core.Application.Queries.Posts;

/// <summary>
/// Reads a post from MongoDB (read store) with Redis cache-aside.
/// PostgreSQL is never touched on the read path.
/// </summary>
public record GetPostQuery(string PostId) : IRequest<PostDto?>;
