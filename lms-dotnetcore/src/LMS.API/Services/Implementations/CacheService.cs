using LMS.API.Configuration;
using LMS.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace LMS.API.Services.Implementations
{
    public class CacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly RedisSettings _redisSettings;
        private readonly ILogger<CacheService> _logger;

        public CacheService(
            IConnectionMultiplexer redis,
            IOptions<RedisSettings> redisSettings,
            ILogger<CacheService> logger)
        {
            _redis = redis;
            _redisSettings = redisSettings.Value;
            _logger = logger;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var db = _redis.GetDatabase();
                var serialized = JsonSerializer.Serialize(value);
                var exp = expiration ?? TimeSpan.FromDays(_redisSettings.CacheExpirationDays);

                await db.StringSetAsync(key, serialized, exp);
                _logger.LogInformation("Cache set for key: {Key} with expiration: {Expiration}", key, exp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key: {Key}", key);
                throw;
            }
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var db = _redis.GetDatabase();
                var value = await db.StringGetAsync(key);

                if (!value.HasValue)
                {
                    _logger.LogInformation("Cache miss for key: {Key}", key);
                    return default;
                }

                var deserialized = JsonSerializer.Deserialize<T>(value.ToString());
                _logger.LogInformation("Cache hit for key: {Key}", key);
                return deserialized;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache for key: {Key}", key);
                return default;
            }
        }

        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                var db = _redis.GetDatabase();
                var deleted = await db.KeyDeleteAsync(key);
                _logger.LogInformation("Cache deleted for key: {Key}", key);
                return deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cache for key: {Key}", key);
                return false;
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                var db = _redis.GetDatabase();
                var exists = await db.KeyExistsAsync(key);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
                return false;
            }
        }
    }
}
