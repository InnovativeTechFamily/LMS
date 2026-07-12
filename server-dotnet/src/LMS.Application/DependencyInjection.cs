using LMS.Application.Services;
using LMS.Application.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Application;

public static class DependencyInjection
{
    /// <summary>Registers the application's use-case services. Infrastructure supplies their dependencies.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ILayoutService, LayoutService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();

        return services;
    }
}
