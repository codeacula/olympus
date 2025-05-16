namespace Olympus.Bot.Discord;

/// <summary>
/// Contains structured logging definitions for the Discord bot worker.
/// </summary>
public static partial class DiscordLogger
{
  [LoggerMessage(
    Level = LogLevel.Information,
    EventId = 0,
    Message = "Discord Gateway is ready.")]
  public static partial void LogDiscordGatewayReady(ILogger logger);

  [LoggerMessage(
      Level = LogLevel.Information,
      EventId = 1,
      Message = "Worker running at: {Time}")]
  public static partial void LogWorkerRunning(
      ILogger logger,
      DateTimeOffset time);

  [LoggerMessage(
      Level = LogLevel.Information,
      EventId = 2,
      Message = "Discord Message Content: {Content}")]
  public static partial void LogMessageContent(
      ILogger logger,
      string content);

  [LoggerMessage(
      Level = LogLevel.Information,
      EventId = 3,
      Message = "Starting Discord Gateway...")]
  public static partial void LogStartingDiscordGateway(ILogger logger);

  [LoggerMessage(
      Level = LogLevel.Information,
      EventId = 4,
      Message = "Discord Gateway disposing...")]
  public static partial void LogDiscordGatewayDisposing(ILogger logger);
}
