using API.Core.Application.Domain.Dto.Post;
using API.Core.Application.Interfaces;
using API.Core.Domain.Entities;
using API.Core.Domain.Interfaces;
using MediatR;

namespace API.Core.Application.Queries.Posts;

public class GetPostQueryHandler : IRequestHandler<GetPostQuery, PostDto?>
{
    private readonly IPostRepository _postRepository;
    private readonly IPostCacheService _cache;

    public GetPostQueryHandler(IPostRepository postRepository, IPostCacheService cache)
    {
        _postRepository = postRepository;
        _cache = cache;
    }

    public async Task<PostDto?> Handle(GetPostQuery query, CancellationToken cancellationToken)
    {
        // Cache-aside — Redis first to avoid a MongoDB round-trip on hot paths.
        Post? cached = await _cache.GetPostAsync(query.PostId, cancellationToken);
        if (cached is not null)
            return ToDto(cached);

        Post? post = await _postRepository.GetByIdAsync(query.PostId, cancellationToken);
        if (post is null)
            return null;

        await _cache.SetPostAsync(post, cancellationToken);
        return ToDto(post);
    }

    private static PostDto ToDto(Post p) =>
        new(p.Id, p.BlogId, p.AuthorId, p.Title, p.Body, p.Tags, p.Comments.Count, p.CreatedAt);
}
