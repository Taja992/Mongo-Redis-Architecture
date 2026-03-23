using API.Core.Application.Domain.Dto.Post;

namespace API.Core.Application.Domain.Dto.Blog;

public record BlogDto(
    string Id,
    string UserId,
    string Title,
    string Description,
    List<string> Tags,
    List<PostDto> Posts,
    DateTime CreatedAt
);
