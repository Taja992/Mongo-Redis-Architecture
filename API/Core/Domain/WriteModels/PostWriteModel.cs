namespace API.Core.Domain.WriteModels;

/// <summary>
/// PostgreSQL write-side record for posts.
/// MongoDB Post is the read-side projection, kept in sync via domain events.
/// </summary>
public class PostWriteModel
{
    /// <summary>
    /// MongoDB ObjectId string, generated in the command handler before writing to either store.
    /// Both databases share this key so the write side and read side are always co-addressable.
    /// </summary>
    public string Id { get; set; } = null!;

    public string BlogId { get; set; } = null!;
    public string AuthorId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;

    /// <summary>
    /// Tags serialized as a JSON array string (e.g. '["dotnet","redis"]').
    /// </summary>
    public string Tags { get; set; } = "[]";

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
