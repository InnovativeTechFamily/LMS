using System.Collections.Concurrent;
using System.Text.Json;
using LMS.Application.Common.Interfaces.Services;

namespace LMS.IntegrationTests.Fakes;

/// <summary>
/// In-memory stand-in for the Redis cache. Serializes values with the same options as
/// <c>RedisCacheService</c> so behaviour (camelCase, <c>[JsonIgnore]</c> on Password) matches.
/// </summary>
public class InMemoryCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, string> _store = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        if (_store.TryGetValue(key, out var json))
            return Task.FromResult(JsonSerializer.Deserialize<T>(json, JsonOptions));
        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        _store[key] = JsonSerializer.Serialize(value, JsonOptions);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _store.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public void Clear() => _store.Clear();
}
