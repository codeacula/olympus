namespace Olympus.Bot.Discord;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
  private readonly ILogger<Worker> _logger = logger;

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      if (_logger.IsEnabled(LogLevel.Information))
      {
        DiscordLogger.LogWorkerRunning(_logger, DateTimeOffset.Now);
      }
      await Task.Delay(1000, stoppingToken);
    }
  }
}
