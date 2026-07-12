namespace LMS.API.Configuration
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int CacheExpirationDays { get; set; } = 7;
    }
}
