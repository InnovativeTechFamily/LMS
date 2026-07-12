namespace LMS.API.Services.Interfaces
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task<T?> GetAsync<T>(string key);
        Task<bool> DeleteAsync(string key);
        Task<bool> ExistsAsync(string key);
    }
}
