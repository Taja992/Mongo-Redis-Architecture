using API.Core.Domain.Exceptions;
// using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Core.Application.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var problemDetails = exception switch
        {
            AppException appEx => new ProblemDetails
            {
                Status = appEx.StatusCode,
                Title = ExceptionHandlingExtensions.GetTitleFromException(appEx),
                Detail = appEx.Message,
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["errorCode"] = appEx.ErrorCode,
                    ["traceId"] = httpContext.TraceIdentifier,
                },
            },
            // Keep this block as part of the mongoredisarchitecture; uncomment when FluentValidation is added.
            // ValidationException validationEx => new ProblemDetails
            // {
            //     Status = 400,
            //     Title = "Validation Error",
            //     Instance = httpContext.Request.Path,
            //     Extensions =
            //     {
            //         ["errors"] = validationEx
            //             .Errors.GroupBy(x => x.PropertyName)
            //             .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray()),
            //         ["traceId"] = httpContext.TraceIdentifier,
            //     },
            // },
            _ => new ProblemDetails
            {
                Status = 500,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred",
                Instance = httpContext.Request.Path,
                Extensions = { ["traceId"] = httpContext.TraceIdentifier },
            },
        };

        // Merge any additional extensions from AppException
        if (exception is AppException appException && appException.Extensions != null)
        {
            foreach (var ext in appException.Extensions)
            {
                problemDetails.Extensions[ext.Key] = ext.Value;
            }
        }

        // Log the error details with structured logging
        logger.LogError(
            exception,
            "Error occurred: {StatusCode} {Title} - {Detail} | Path: {Path} | TraceId: {TraceId}",
            problemDetails.Status,
            problemDetails.Title,
            problemDetails.Detail,
            httpContext.Request.Path,
            httpContext.TraceIdentifier
        );

        httpContext.Response.StatusCode = problemDetails.Status ?? 500;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
