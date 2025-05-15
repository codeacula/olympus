using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Olympus.Tests.Integration;

/// <summary>
/// Base class for integration tests that configures a test host with mocked services
/// </summary>
public abstract class BaseIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
  protected readonly WebApplicationFactory<Program> Factory;
  protected readonly HttpClient Client;

  protected BaseIntegrationTest(WebApplicationFactory<Program> factory)
  {
    Factory = factory.WithWebHostBuilder(builder =>
    {
      _ = builder.ConfigureServices(services =>
          {
            // Configure test services here
            // For example, replace real external services with mocks
            ConfigureTestServices(services);
          });
    });

    Client = Factory.CreateClient();
  }

  /// <summary>
  /// Override this method to configure specific services for your test
  /// </summary>
  /// <param name="services"></param>
  protected virtual void ConfigureTestServices(IServiceCollection services)
  {
    // Default implementation does nothing
  }
}
