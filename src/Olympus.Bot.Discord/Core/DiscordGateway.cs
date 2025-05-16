using Microsoft.Extensions.Options;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace Olympus.Bot.Discord.Core;

public sealed class DiscordGateway : IDisposable
{
  private readonly ApplicationCommandService<ApplicationCommandContext> _applicationCommandService = new();
  private readonly ApplicationCommandServiceManager _applicationCommandServiceManager = new();
  private readonly DiscordSettings _discordSettings;
  private readonly GatewayClient _gatewayClient;
  private readonly ILogger<DiscordGateway> _logger;

  public DiscordGateway(IOptions<DiscordSettings> discordSettings, ILogger<DiscordGateway> logger)
  {
    _discordSettings = discordSettings.Value;
    _logger = logger;

    _gatewayClient = new GatewayClient(new BotToken(_discordSettings.BotToken), new()
    {
      Intents = GatewayIntents.AllNonPrivileged | GatewayIntents.MessageContent
    });
    _gatewayClient.Ready += OnReadyAsync;
    _gatewayClient.Log += OnLogAsync;
    _gatewayClient.MessageCreate += OnMessageCreate;
    _gatewayClient.InteractionCreate += OnNewInteractionAsync;

    _applicationCommandService.AddModules(typeof(Program).Assembly);

    _applicationCommandServiceManager.AddService(_applicationCommandService);
  }

  private ValueTask OnMessageCreate(Message message)
  {
    DiscordLogger.LogMessageContent(_logger, message.Content);
    return default;
  }

  private async ValueTask OnNewInteractionAsync(Interaction interaction)
  {
    if (interaction is not ApplicationCommandInteraction commandInteraction)
    {
      return;
    }

    var context = new ApplicationCommandContext(commandInteraction, _gatewayClient);
    var result = await _applicationCommandService.ExecuteAsync(context);

    if (result is not IFailResult failResult)
    {
      return;
    }

    try
    {
      await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
    }
    catch
    {
    }
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    DiscordLogger.LogStartingDiscordGateway(_logger);

    var foundCommands = await _applicationCommandServiceManager.CreateCommandsAsync(_gatewayClient.Rest, _gatewayClient.Id, cancellationToken: cancellationToken);

    if (foundCommands is not null)
    {
      foreach (var command in foundCommands)
      {
        DiscordLogger.LogMessageContent(_logger, $"Found command: {command.Name}");
      }
    }

    await _gatewayClient.StartAsync(null, cancellationToken);
  }

  private async ValueTask OnReadyAsync(ReadyEventArgs args)
  {
    DiscordLogger.LogDiscordGatewayReady(_logger);

    //var foundCommands = await applicationCommandService.CreateCommandsAsync(_gatewayClient.Rest, _gatewayClient.Id, includeGuildCommands: true);
  }

  private ValueTask OnLogAsync(LogMessage message)
  {
    DiscordLogger.LogMessageContent(_logger, message.ToString());

    return default;
  }

  public void Dispose()
  {
    _gatewayClient.Dispose();
    DiscordLogger.LogDiscordGatewayDisposing(_logger);
  }
}
