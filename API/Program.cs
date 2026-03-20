using API.Api;
using API.Application.Extensions;
using API.Core.Application.Exceptions;
using API.Core.Configuration;
using API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (context, services, config) =>
        config
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
);

// Configure IOptions settings.
builder.Services.Configure<AppOptions>(builder.Configuration.GetSection("AppOptions"));

// Load settings for use in Program.cs.
var appOptions = builder.Configuration.GetSection("AppOptions").Get<AppOptions>()!;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "FrontendPolicy",
        policy =>
        {
            policy
                .WithOrigins(appOptions.FrontendUrl)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

// Minimal DbContext setup for template startup; replace with Npgsql/SqlServer later.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
    options.UseInMemoryDatabase("TemplateDb");
});

// TODO: Add these when extension methods/types exist in template.
builder.Services.AddExceptionHandling();

// builder.Services.AddJwtAuthentication(jwtSettings);
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// builder.Services.AddIdentityServices();
// builder.Services.AddRateLimiting();

var app = builder.Build();

app.UseRouting();
app.UseExceptionHandler();

// Scalar/OpenAPI docs available in dev/staging like your full app pattern.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");

app.MapHealthChecks("/health").AllowAnonymous();
app.MapTemplatesApi();

app.Run();

public partial class Program { }
