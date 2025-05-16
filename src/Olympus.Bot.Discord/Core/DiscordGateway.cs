using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;

namespace Olympus.Bot.Discord.Core;

public sealed partial class DiscordGateway(DiscordSettings discordSettings, ILogger<DiscordGateway> logger)
{
  private readonly DiscordSettings _discordSettings = discordSettings;
  private readonly GatewayClient _gatewayClient = new(new BotToken(discordSettings.BotToken));
  private readonly ILogger<DiscordGateway> _logger = logger;


  public Task StartAsync(CancellationToken cancellationToken)
  {
    _gatewayClient.MessageCreated += OnMessageCreated;

    _logger.LogInformation("Starting Discord Gateway...");

    return _gatewayClient.StartAsync(cancellationToken);
  }

  [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Discord Gateway is ready.")]
  private static partial void LogDiscordGatewayReady(ILogger logger);
}
