using API.Infrastructure;

namespace IntegrationTests.Seeders;

public class TestSeeder(AppDbContext db)
{
    public async Task InitAsync()
    {
        // TODO: seed test data here
        await Task.CompletedTask;
    }
}
