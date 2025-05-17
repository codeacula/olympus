using NetCord.Services.ApplicationCommands;
using Olympus.Application.Ai.Commands.TestInteraction;
using Olympus.Application.Common.Messaging;
using Olympus.Application.Common.Types;

namespace Olympus.Bot.Discord.Modules;

public abstract partial class TestInteractionModule(
  IOlympusDispatcher dispatcher,
  ILogger<TestInteractionModule> logger) : BaseInteractionModule<TestInteractionModule>(dispatcher, logger)
{
  [SlashCommand("testinteraction", "Test Olympus")]
  public async Task<string> TestInteractionAsync([SlashCommandParameter(Description = "The text to interact with")] string interactionText)
  {
    var command = new TestInteractionCommand(interactionText);

    var result = await Dispatcher.DispatchCommandAsync(command);

    switch (result)
    {
      case OlympusResult<TestAiInteractionCommandResult, OlympusError>.Success success:
        LogCommandExecuted(Logger, interactionText);
        return success.Value.ReplayTest;

      case OlympusResult<TestAiInteractionCommandResult, OlympusError>.Failure failure:
        LogCommandError(Logger, failure.Error.Message!);
        return $"Error: {failure.Error.Message}";

      default:
        return "Unknown result type";
    }
  }

  [LoggerMessage(
    Level = LogLevel.Information,
    EventId = 0,
    Message = "Test command executed with text: {InteractionText}")]
  public static partial void LogCommandExecuted(ILogger logger, string interactionText);

  [LoggerMessage(
    Level = LogLevel.Error,
    EventId = 1,
    Message = "Test command result: {Result}")]
  public static partial void LogCommandError(ILogger logger, string result);
}
