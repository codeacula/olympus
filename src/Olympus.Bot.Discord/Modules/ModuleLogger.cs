namespace Olympus.Bot.Discord.Modules;

public static partial class ModuleLogger
{
  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Command {CommandName} executed with text: {InteractionText}")]
  public static partial void LogCommandExecuted(ILogger logger, string commandName, string interactionText);

  [LoggerMessage(
      Level = LogLevel.Error,
      Message = "Command returned an error: {Result}")]
  public static partial void LogCommandError(ILogger logger, string result);

  [LoggerMessage(
    Level = LogLevel.Warning,
    Message = "Command returned failure: {message}"
  )]
  public static partial void LogCommandFailure(ILogger logger, string? message);
}
