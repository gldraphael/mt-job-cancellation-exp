using System;
using System.Threading.Tasks;

namespace WorkerApp.Infrastructure.Redis
{
    public interface IRedisService
    {
        Task AddKeyAsync(string key, TimeSpan expiry);
        Task<bool> KeyExistsAsync(string key);
        Task RemoveKeyAsync(string uniqueness);
    }
}