using LMS.API.Configuration;
using LMS.API.Services.Interfaces;
using LMS.API.Services.Implementations;
using MongoDB.Driver;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace LMS.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
           
            // MongoDB Configuration
            var mongoSettings = new MongoDbSettings();
            configuration.GetSection("MongoDB").Bind(mongoSettings);
            services.AddSingleton(mongoSettings);

            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoSettings.DatabaseName);
            services.AddSingleton(mongoDatabase);

            // MongoDB Collections
            services.AddSingleton(mongoDatabase.GetCollection<LMS.API.Models.Domain.User>("users"));
            services.AddSingleton(mongoDatabase.GetCollection<LMS.API.Models.Domain.Course>("courses"));
            services.AddSingleton(mongoDatabase.GetCollection<LMS.API.Models.Domain.Order>("orders"));
            services.AddSingleton(mongoDatabase.GetCollection<LMS.API.Models.Domain.Notification>("notifications"));
            services.AddSingleton(mongoDatabase.GetCollection<LMS.API.Models.Domain.Layout>("layouts"));

            // Redis Configuration
            var redisSettings = new RedisSettings();
            configuration.GetSection("Redis").Bind(redisSettings);
            services.AddSingleton(redisSettings);

            var redisConnection = ConnectionMultiplexer.Connect(redisSettings.ConnectionString);
            services.AddSingleton(redisConnection);

            // Settings
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));
            services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
            services.Configure<EmailSettings>(configuration.GetSection("Email"));
            services.Configure<CorsSettings>(configuration.GetSection("Cors"));

            // Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<ILayoutService, LayoutService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<ICacheService, CacheService>();

            // Replace the ambiguous AddAutoMapper call with the following line in AddApplicationServices:
            services.AddAutoMapper(typeof(Program).Assembly);
            return services;
        }

        public static IServiceCollection AddAuthenticationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.GetSection("Jwt").Bind(jwtSettings);

            var key = Encoding.ASCII.GetBytes(jwtSettings.AccessTokenSecret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }

        public static IServiceCollection AddCorsServices(
            this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:3000",
                            "https://lms-ivory-one.vercel.app"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            return services;
        }

        public static IServiceCollection AddSwagger(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "LMS API",
                    Version = "v1",
                    Description = "Learning Management System API",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "InnovativeTechFamily",
                        Url = new Uri("https://github.com/InnovativeTechFamily")
                    }
                });

                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            return services;
        }
    }
}
