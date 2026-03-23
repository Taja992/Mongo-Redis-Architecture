namespace API.Core.Application.Domain.Dto.Post;

public record CreatePostDto(string AuthorId, string Title, string Body, List<string> Tags);
