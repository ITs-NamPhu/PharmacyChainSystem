public interface IRedisCacheService
{
    Task SetAsync<T>(string key, T data, TimeSpan? expiration = null);
    Task<T> GetAsync<T>(string key);
    Task RemoveAsync(string key);
}