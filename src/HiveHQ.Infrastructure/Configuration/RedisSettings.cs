namespace HiveHQ.Infrastructure.Configuration;

public class RedisSettings
{
    public const string SectionName = "RedisConnection"; // Matches the JSON key
    public string ConnectionString { get; set; } = "localhost:6379";
}
