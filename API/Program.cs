using API.Api;
using API.Core.Application.Exceptions;
using API.Core.Application.Extensions;
using API.Core.Application.Interfaces;
using API.Core.Configuration;
using API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
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
builder.Services.Configure<WriteDbOptions>(builder.Configuration.GetSection("WriteDb"));
builder.Services.Configure<MongoDbOptions>(builder.Configuration.GetSection("MongoDB"));
builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection("Redis"));

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

WriteDbOptions writeDbOptions =
    builder.Configuration.GetSection("WriteDb").Get<WriteDbOptions>()
    ?? throw new InvalidOperationException("WriteDb configuration section is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
    options.UseNpgsql(writeDbOptions.ConnectionString);
});

// TODO: Add these when extension methods/types exist in mongoredisarchitecture.
builder.Services.AddExceptionHandling();

// MongoDB
MongoDbOptions mongoDbOptions =
    builder.Configuration.GetSection("MongoDB").Get<MongoDbOptions>()
    ?? throw new InvalidOperationException("MongoDB configuration section is not configured.");

builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoDbOptions.ConnectionString));

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDbOptions.Database)
);

// Redis
RedisOptions redisOptions =
    builder.Configuration.GetSection("Redis").Get<RedisOptions>()
    ?? throw new InvalidOperationException("Redis configuration section is not configured.");

builder.Services.AddRedis(redisOptions);

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
app.MapBlogApi();
app.MapPostApi();

using (IServiceScope scope = app.Services.CreateScope())
{
    // Ensure the PostgreSQL schema exists on first run.
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();

    IPostSearchService search = scope.ServiceProvider.GetRequiredService<IPostSearchService>();
    await search.EnsureIndexAsync();
}

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();
        string? address = app.Urls.FirstOrDefault();
        logger.LogInformation("Scalar docs: {Address}/scalar", address);
    });
}

app.Run();

public partial class Program { }
