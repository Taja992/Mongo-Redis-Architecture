using API.Core.Application.Interfaces;
using API.Core.Application.Services;
using API.Core.Domain.Interfaces;
using API.Infrastructure.Repositories;

namespace API.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //services.AddScoped<ICurrentContext, CurrentContext>();

        // Application Services
        services.AddScoped<ITemplateService, TemplateService>();
        // services.AddScoped<IAzureBlobService, AzureBlobService>();
        // services.AddScoped<IUserRegistrationService, UserRegistrationService>();
        // services.AddScoped<IUserService, UserService>();
        // services.AddScoped<IStripeService, StripeService>();

        // URL Validation
        //services.AddSingleton<IUrlValidator, UrlBlocklist>();

        // TimeProvider
        services.AddSingleton(TimeProvider.System);

        // Auth Services
        // services.AddScoped<ITokenClaimsService, JwtTokenClaimService>();
        // services.AddScoped<IOAuthCoordinator, OAuthCoordinator>();
        // services.AddScoped<IGoogleService, GoogleService>();
        // services.AddScoped<IYouTubeService, YouTubeService>();
        // services.AddScoped<ITwitchService, TwitchService>();
        // services.AddScoped<ITwitterService, TwitterService>();
        // services.AddScoped<IOAuthService, OAuthService>();
        // services.AddScoped<IUserDataExportService, UserDataExportService>();

        // FleuntValidation the RegisterRequestValidator is just a marker because the method uses <T>
        // so any validator will work.
        //services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        // services.AddValidation(options =>
        // {
        //     options.MaxDepth = 10;
        // });

        // Email services
        // services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
        // services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        // services.AddScoped<IUserRepository, UserRepository>();
        // services.AddScoped<IUserDataExportRepository, UserDataExportRepository>();

        // Interceptors
        //services.AddScoped<OwnershipInterceptor>();

        // Seeder
        //services.AddScoped<DbSeeder>();
        return services;
    }
}
