using API.Infrastructure;
using IntegrationTests.Seeders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.PostgreSql;

namespace IntegrationTests.WebApplicationFactories;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("Template-Integration")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace DB context
            var dbContextDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>)
            );

            services.Remove(dbContextDescriptor!);

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString())
            );

            // TODO: Replace stub services when interfaces are added
            // e.g. services.Replace(ServiceDescriptor.Scoped<IEmailService, TestEmailService>());

            // Use null logger for tests
            services.AddSingleton<ILoggerFactory, NullLoggerFactory>();

            // Ensure tests don't enforce the global fallback auth policy so public endpoints remain accessible
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = null;
            });

            // Seed DB
            services.AddScoped<TestSeeder>();
        });

        builder.UseEnvironment("Development");
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        var testSeeder = scope.ServiceProvider.GetRequiredService<TestSeeder>();
        await testSeeder.InitAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await base.DisposeAsync().AsTask();
    }
}
