using API.Core.Application.Commands.Posts;
using API.Core.Application.Domain.Interfaces;
using API.Core.Application.Interfaces;
using API.Core.Application.Services;
using API.Core.Configuration;
using API.Core.Domain.Interfaces;
using API.Infrastructure.Cache;
using API.Infrastructure.Repositories;
using API.Infrastructure.Search;
using API.Infrastructure.SqlRepositories;
using StackExchange.Redis;

namespace API.Core.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR — registers all handlers in this assembly automatically.
        // Why RegisterServicesFromAssemblyContaining<CreatePostCommand>()? CreatePostCommand
        // is in API.Core.Application.Commands.Posts. MediatR scans the whole assembly containing
        // that type — which is the API project — so it finds every handler including the Infrastructure-layer
        //  event handlers (PostCreatedEventHandler, etc.) all in one call.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreatePostCommand>());

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

        // CQRS write-side repository (PostgreSQL).
        services.AddScoped<IPostWriteRepository, SqlPostWriteRepository>();

        // Interceptors
        //services.AddScoped<OwnershipInterceptor>();

        // Seeder
        //services.AddScoped<DbSeeder>();
        return services;
    }

    public static IServiceCollection AddRedis(
        this IServiceCollection services,
        RedisOptions options
    )
    {
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(options.ConnectionString)
        );

        return services;
    }
}
