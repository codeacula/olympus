namespace Olympus.Bot.Discord.Core;

public class DiscordBotWorker(DiscordGateway discordGateway, ILogger<DiscordBotWorker> logger) : BackgroundService
{
  private readonly DiscordGateway _discordGateway = discordGateway;
  private readonly ILogger<DiscordBotWorker> _logger = logger;

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await _discordGateway.StartAsync(stoppingToken);

    while (!stoppingToken.IsCancellationRequested)
    {
    }
  }
}
