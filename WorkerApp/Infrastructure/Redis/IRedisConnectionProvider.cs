using StackExchange.Redis;

namespace WorkerApp.Infrastructure.Redis
{
    /// <summary>
    /// Provides a reference to the redis connection.
    /// </summary>
    public interface IRedisConnectionProvider
    {
        IConnectionMultiplexer Connection { get; }
    }
}