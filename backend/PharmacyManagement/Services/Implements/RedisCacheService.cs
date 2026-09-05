using StackExchange.Redis;
using PharmacyManagement.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task SetAsync<T>(string key, T data, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry };
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(data), options);
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);
        return data == null ? default : JsonSerializer.Deserialize<T>(data);
    }

    public async Task RemoveAsync(string key) => await _cache.RemoveAsync(key);
}