namespace API.Core.Application.Domain.Dto.Post;

public record PostSearchResultDto(
    string Id,
    string BlogId,
    string Title,
    string Snippet,
    List<string> Tags
);
