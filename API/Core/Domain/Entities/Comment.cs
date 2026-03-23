using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Core.Domain.Entities;

public class Comment
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string AuthorId { get; set; } = null!;

    public string Body { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
