namespace Library.Options;

public class MongoDbOptions
{
    public required string ConnectionString { get; init; }
    public required string DbName { get; init; }
}