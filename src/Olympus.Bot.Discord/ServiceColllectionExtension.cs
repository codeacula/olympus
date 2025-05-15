using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Rest;

namespace Olympus.Bot.Discord;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddDiscordServices(this IServiceCollection services)
  {
    _ = services
      .AddDiscordGateway(options => options.Intents =
        GatewayIntents.GuildMessages
        | GatewayIntents.DirectMessages
        | GatewayIntents.MessageContent
        | GatewayIntents.DirectMessageReactions
        | GatewayIntents.GuildMessageReactions
      )
      .AddDiscordRest()
      .AddGatewayEventHandlers(typeof(Program).Assembly);
    return services;
  }

  public static IHost AddDiscordServices(this IHost host)
  {
    _ = host.UseGatewayEventHandlers();
    return host;
  }
}
