namespace LMS.API.Configuration
{
    public class JwtSettings
    {
        public string AccessTokenSecret { get; set; } = string.Empty;
        public string RefreshTokenSecret { get; set; } = string.Empty;
        public string ActivationTokenSecret { get; set; } = string.Empty;
        public int AccessTokenExpirationMinutes { get; set; } = 5;
        public int RefreshTokenExpirationDays { get; set; } = 3;
        public int ActivationTokenExpirationMinutes { get; set; } = 5;
    }
}
