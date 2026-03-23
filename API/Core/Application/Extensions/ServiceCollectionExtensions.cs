using API.Core.Application.Domain.Interfaces;
using API.Core.Application.Interfaces;
using API.Core.Application.Services;
using API.Core.Domain.Interfaces;
using API.Infrastructure.Cache;
using API.Infrastructure.Repositories;
using API.Infrastructure.Search;
using StackExchange.Redis;

namespace API.Core.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //services.AddScoped<ICurrentContext, CurrentContext>();

        // Application Services
        services.AddScoped<IMongoRedisArchitectureService, MongoRedisArchitectureService>();

        // TimeProvider
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IBlogService, BlogService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IPostCacheService, RedisPostCacheService>();
        services.AddScoped<IRateLimitService, RedisRateLimitService>();
        services.AddScoped<IPostSearchService, RedisPostSearchService>();

        // Auth Services

        // FleuntValidation the RegisterRequestValidator is just a marker because the method uses <T>
        // so any validator will work.
        //services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        // services.AddValidation(options =>
        // {
        //     options.MaxDepth = 10;
        // });

        // Email services
        // services.AddSingleton<IEmailMongoRedisArchitectureService, EmailMongoRedisArchitectureService>();
        // services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IMongoRedisArchitectureRepository, MongoRedisArchitectureRepository>();
        services.AddScoped<IBlogRepository, MongoBlogRepository>();
        services.AddScoped<IPostRepository, MongoPostRepository>();

        // Interceptors
        //services.AddScoped<OwnershipInterceptor>();

        // Seeder
        //services.AddScoped<DbSeeder>();
        return services;
    }

    public static IServiceCollection AddRedis(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string connectionString =
            configuration["Redis:ConnectionString"]
            ?? throw new InvalidOperationException("Redis:ConnectionString is not configured.");

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(connectionString)
        );

        return services;
    }
}
