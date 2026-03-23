using System.Text.RegularExpressions;
using API.Core.Application.Domain.Dto.Post;
using API.Core.Application.Interfaces;
using API.Core.Domain.Entities;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;

namespace API.Infrastructure.Search;

public class RedisPostSearchService : IPostSearchService
{
    private readonly IDatabase _db;
    private const string IndexName = "idx:posts";

    public RedisPostSearchService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task EnsureIndexAsync(CancellationToken cancellationToken = default)
    {
        var ft = _db.FT();

        try
        {
            // If the index already exists this throws — that is expected and safe to ignore.
            await ft.CreateAsync(
                IndexName,
                new FTCreateParams().On(IndexDataType.JSON).Prefix("search:post:"),
                new Schema()
                    .AddTextField(new FieldName("$.Title", "title"))
                    .AddTextField(new FieldName("$.Body", "body"))
                    .AddTagField(new FieldName("$.Tags[*]", "tags"))
            );
        }
        catch (RedisServerException ex) when (ex.Message.Contains("Index already exists"))
        {
            // Index is already there — nothing to do.
        }
    }

    public async Task IndexPostAsync(Post post, CancellationToken cancellationToken = default)
    {
        await _db.JSON().SetAsync(SearchKey(post.Id), "$", post);
    }

    public async Task RemovePostAsync(string postId, CancellationToken cancellationToken = default)
    {
        await _db.KeyDeleteAsync(SearchKey(postId));
    }

    public async Task<List<PostSearchResultDto>> SearchAsync(
        string query,
        int limit = 10,
        CancellationToken cancellationToken = default
    )
    {
        var ft = _db.FT();

        string escaped = EscapeQuery(query);
        if (string.IsNullOrWhiteSpace(escaped))
            return [];

        string ftQuery = $"@title:({escaped}) | @body:({escaped})";

        SearchResult results = await ft.SearchAsync(IndexName, new Query(ftQuery).Limit(0, limit));

        List<PostSearchResultDto> posts = [];

        foreach (Document doc in results.Documents)
        {
            Post? post = null;

            // Some Redis/RediSearch setups return full JSON payload in "$".
            // Others are effectively key-only and require an explicit JSON.GET by doc.Id.
            string? raw = doc["$"];
            if (!string.IsNullOrWhiteSpace(raw))
            {
                post = System.Text.Json.JsonSerializer.Deserialize<Post>(raw);
            }

            if (post is null)
            {
                post = await _db.JSON().GetAsync<Post>(doc.Id);
            }

            if (post is null)
                continue;

            posts.Add(ToSearchResult(post));
        }

        return posts;
    }

    private static PostSearchResultDto ToSearchResult(Post p) =>
        new(p.Id, p.BlogId, p.Title, p.Body.Length > 200 ? p.Body[..200] + "..." : p.Body, p.Tags);

    private static string SearchKey(string postId) => $"search:post:{postId}";

    private static string EscapeQuery(string q)
    {
        return Regex.Replace(
            q.Trim(),
            @"[,.<>{}\[\]""':;!@#$%^&*()\-+=~|/\\]",
            m => $"\\{m.Value}"
        );
    }
}
