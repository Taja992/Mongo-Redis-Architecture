namespace API.Core.Application.Domain.Dto.Post;

public record UpdatePostDto(string? Title, string? Body, List<string>? Tags);
