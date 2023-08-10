using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace WorkerApp.Infrastructure.Redis
{
    public class RedisService : IRedisService, IJobCache
    {
        private readonly IDatabaseAsync database;

        public RedisService(IRedisConnectionProvider redisConnectionProvider)
        {
            var redis = redisConnectionProvider.Connection;
            database = redis.GetDatabase();
        }

        public async Task<bool> KeyExistsAsync(string key) =>
            await database.KeyExistsAsync(key);

        public async Task AddKeyAsync(string key, TimeSpan expiry) =>
            await database.StringSetAsync(key, true, expiry);

        public async Task RemoveKeyAsync(string uniqueness) =>
            await database.KeyDeleteAsync(uniqueness);

        public async Task AddCancellationRequest(Guid jobId) =>
            await database.StringSetAsync($"job-cancel-{jobId}", true, TimeSpan.FromMinutes(1));

        public async Task<bool> CheckForCancellation(Guid jobId) => await database.KeyExistsAsync($"job-cancel-{jobId}");
    }
}
