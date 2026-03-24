namespace API.Core.Configuration;

public class MongoDbOptions
{
    public required string ConnectionString { get; set; }
    public required string Database { get; set; }
}
