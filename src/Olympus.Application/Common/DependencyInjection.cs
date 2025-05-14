using Microsoft.Extensions.DependencyInjection;

namespace Olympus.Application.Common;

public static class DependencyInjection
{
  /// <summary>
  /// Registers Application layer services
  /// </summary>
  /// <param name="services"></param>
  /// <returns></returns>
  public static IServiceCollection AddOlympusApplication(this IServiceCollection services)
  {
    // TODO: Register handlers and abstractions here
    return services;
  }
}
