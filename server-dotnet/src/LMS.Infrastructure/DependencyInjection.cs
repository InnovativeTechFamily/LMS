using LMS.Application.Common.Interfaces.Persistence;
using LMS.Application.Common.Interfaces.Services;
using LMS.Infrastructure.Caching;
using LMS.Infrastructure.Configuration;
using LMS.Infrastructure.Email;
using LMS.Infrastructure.Identity;
using LMS.Infrastructure.Media;
using LMS.Infrastructure.Payments;
using LMS.Infrastructure.Persistence;
using LMS.Infrastructure.Persistence.Repositories;
using LMS.Infrastructure.Video;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace LMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Options
        services.Configure<MongoSettings>(configuration.GetSection(MongoSettings.SectionName));
        services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.SectionName));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));
        services.Configure<StripeSettings>(configuration.GetSection(StripeSettings.SectionName));
        services.Configure<VdoCipherSettings>(configuration.GetSection(VdoCipherSettings.SectionName));
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));

        // Persistence (MongoDB)
        services.AddSingleton<MongoContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<ILayoutRepository, LayoutRepository>();

        // Redis
        var redisConnection = configuration.GetSection(RedisSettings.SectionName)
            .Get<RedisSettings>()?.ConnectionString ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));
        services.AddSingleton<ICacheService, RedisCacheService>();

        // Identity / security
        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<IIdGenerator, ObjectIdGenerator>();

        // Integrations
        services.AddSingleton<IMediaStorage, CloudinaryMediaStorage>();
        services.AddSingleton<IPaymentService, StripePaymentService>();
        services.AddSingleton<IEmailService, SmtpEmailService>();
        services.AddHttpClient<IVideoService, VdoCipherVideoService>();

        return services;
    }
}
