namespace API.Core.Application.Domain.Dto.Post;

public record PostDto(
    string Id,
    string BlogId,
    string AuthorId,
    string Title,
    string Body,
    List<string> Tags,
    int CommentCount,
    DateTime CreatedAt
);
