using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace WorkerApp.Infrastructure.Redis
{
    public class RedisConnectionProvider : IRedisConnectionProvider
    {
        private static readonly object connectionMuxLock = new object();
        private static ConnectionMultiplexer connectionMultiplexer;

        public IConnectionMultiplexer Connection => connectionMultiplexer;

        public RedisConnectionProvider(IOptions<RedisConfig> options)
        {
            if(Connection is null)
            {
                lock(connectionMuxLock)
                {
                    if(Connection is null)
                    {
                        var config_options = ConfigurationOptions.Parse(options.Value.ConnectionString ?? $"{options.Value.Host}:{options.Value.Port}");
                        config_options.SyncTimeout = options.Value.Timeout;
                        connectionMultiplexer = ConnectionMultiplexer.Connect(config_options);
                    }
                }
            }
        }
    }
}
