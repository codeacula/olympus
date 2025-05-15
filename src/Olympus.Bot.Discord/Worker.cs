namespace Olympus.Bot.Discord;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
  private readonly ILogger<Worker> _logger = logger;

  private static readonly Action<ILogger, DateTimeOffset, Exception?> LogWorkerRunning =
      LoggerMessage.Define<DateTimeOffset>(
          LogLevel.Information,
          new EventId(1, nameof(LogWorkerRunning)),
          "Worker running at: {Time}");

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      if (_logger.IsEnabled(LogLevel.Information))
      {
        LogWorkerRunning(_logger, DateTimeOffset.Now, null);
      }
      await Task.Delay(1000, stoppingToken);
    }
  }
}
