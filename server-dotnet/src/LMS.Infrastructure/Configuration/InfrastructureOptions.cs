namespace LMS.Infrastructure.Configuration;

public class MongoSettings
{
    public const string SectionName = "Mongo";
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string Database { get; set; } = "lms";
}

public class RedisSettings
{
    public const string SectionName = "Redis";
    public string ConnectionString { get; set; } = "localhost:6379";
}

public class JwtSettings
{
    public const string SectionName = "Jwt";
    public string AccessTokenSecret { get; set; } = string.Empty;
    public string RefreshTokenSecret { get; set; } = string.Empty;
    public string ActivationSecret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "LMS";
    public string Audience { get; set; } = "LMS";

    /// <summary>Access token lifetime in minutes (default 5, matching the Node server).</summary>
    public int AccessTokenExpireMinutes { get; set; } = 5;

    /// <summary>Refresh token lifetime in days (default 3).</summary>
    public int RefreshTokenExpireDays { get; set; } = 3;

    /// <summary>Activation token lifetime in minutes (default 5).</summary>
    public int ActivationTokenExpireMinutes { get; set; } = 5;
}

public class CloudinarySettings
{
    public const string SectionName = "Cloudinary";
    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
}

public class StripeSettings
{
    public const string SectionName = "Stripe";
    public string SecretKey { get; set; } = string.Empty;
    public string PublishableKey { get; set; } = string.Empty;
}

public class VdoCipherSettings
{
    public const string SectionName = "VdoCipher";
    public string ApiSecret { get; set; } = string.Empty;
}

public class EmailSettings
{
    public const string SectionName = "Email";
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromName { get; set; } = "LMS";
    public string FromAddress { get; set; } = string.Empty;
}
