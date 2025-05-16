namespace Olympus.Bot.Discord;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddDiscordServices(this IServiceCollection services)
  {
    return services;
  }

  public static IHost AddDiscordServices(this IHost host)
  {
    return host;
  }
}
