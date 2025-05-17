namespace Olympus.Bot.Discord.Modules;

public static partial class ModuleLogger
{
  [LoggerMessage(
      Level = LogLevel.Information,
      EventId = 0,
      Message = "Command {CommandName} executed with text: {InteractionText}")]
  public static partial void LogCommandExecuted(ILogger logger, string commandName, string interactionText);

  [LoggerMessage(
      Level = LogLevel.Error,
      EventId = 1,
      Message = "Command returned an error: {Result}")]
  public static partial void LogCommandError(ILogger logger, string result);
}
