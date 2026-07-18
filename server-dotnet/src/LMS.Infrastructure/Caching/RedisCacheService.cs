using System.Text.Json;
using LMS.Application.Common.Interfaces.Services;
using StackExchange.Redis;

namespace LMS.Infrastructure.Caching;

/// <summary>Redis-backed implementation of <see cref="ICacheService"/> using JSON values.</summary>
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    public RedisCacheService(IConnectionMultiplexer redis) => _redis = redis;

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var value = await _redis.GetDatabase().StringGetAsync(key);
        return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value!, JsonOptions);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        return _redis.GetDatabase().StringSetAsync(key, json, expiry);
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
        => _redis.GetDatabase().KeyDeleteAsync(key);
}
