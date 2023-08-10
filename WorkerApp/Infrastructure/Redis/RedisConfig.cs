namespace WorkerApp.Infrastructure.Redis
{
    public class RedisConfig
    {
        public string Host { get; set; } = "localhost";
        public string Port { get; set; } = "6379";
        public string? ConnectionString { get; set; }
        public int Timeout { get; set; } = 10 * 60_000; // default timeout is 10 mins
    }
}
