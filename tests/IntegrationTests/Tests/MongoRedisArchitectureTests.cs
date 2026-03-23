using System.Net;
using FluentAssertions;
using IntegrationTests.WebApplicationFactories;
using Xunit;

namespace IntegrationTests.Tests;

public class MongoRedisArchitectureTests(CustomWebApplicationFactory<Program> factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        var response = await Client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
