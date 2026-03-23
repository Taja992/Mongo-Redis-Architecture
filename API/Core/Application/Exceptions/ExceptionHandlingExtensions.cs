using API.Core.Domain.Exceptions;
using API.Core.Domain.Exceptions.Validation;

namespace API.Core.Application.Exceptions;

public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        return services;
    }

    public static string GetTitleFromException(AppException ex) =>
        ex switch
        {
            MongoRedisArchitectureException => "MongoRedisArchitecture",
            // Uncomment and customize as exception hierarchy grows.
            // NotFoundException => "Not Found",
            // ConflictException => "Already Exists",
            RateLimitExceededException => "Rate Limit Exceeded",
            _ => ex.StatusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                409 => "Conflict",
                422 => "Validation Error",
                429 => "Rate Limit Exceeded",
                _ => "Internal Server Error",
            },
        };
}
