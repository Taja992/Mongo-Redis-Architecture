namespace API.Core.Application.Domain.Dto.Blog;

public record CreateBlogDto(string UserId, string Title, string Description, List<string> Tags);
