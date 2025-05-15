namespace Olympus.Bot.Discord;

/// <summary>
/// Contains structured logging definitions for the Discord bot worker.
/// </summary>
public static partial class DiscordLogger
{
  [LoggerMessage(
      Level = LogLevel.Information,
      EventId = 1,
      Message = "Worker running at: {Time}")]
  public static partial void LogWorkerRunning(
      ILogger logger,
      DateTimeOffset time);
}
