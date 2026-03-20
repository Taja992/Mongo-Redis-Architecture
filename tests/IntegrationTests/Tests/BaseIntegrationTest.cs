using IntegrationTests.WebApplicationFactories;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegrationTests.Tests;

public class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory<Program> Factory;

    public BaseIntegrationTest(CustomWebApplicationFactory<Program> factory)
    {
        Factory = factory;
        Client = factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }
}
