namespace Olympus.Bot.Discord.Core;

public class DiscordBotWorker(DiscordGateway discordGateway, ILogger<DiscordBotWorker> logger) : BackgroundService
{
  private readonly ILogger<DiscordBotWorker> _logger = logger;

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {

    while (!stoppingToken.IsCancellationRequested)
    {
    }
  }
}
