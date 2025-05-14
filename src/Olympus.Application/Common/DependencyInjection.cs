using Microsoft.Extensions.DependencyInjection;

namespace Olympus.Application.Common;

public static class DependencyInjection
{
  // Registers Application layer services
  public static IServiceCollection AddOlympusApplication(this IServiceCollection services)
  {
    // TODO: Register handlers and abstractions here
    return services;
  }
}
