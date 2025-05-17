using NetCord.Services.ApplicationCommands;
using Olympus.Application.Ai.Commands.TestInteractionCommand;
using Olympus.Application.Common.Messaging;

namespace Olympus.Bot.Discord.Commands;

public partial class TestInteractionModule(
  IOlympusDispatcher olympusDispatcher,
  ILogger<TestInteractionModule> logger) : ApplicationCommandModule<ApplicationCommandContext>
{
  private readonly IOlympusDispatcher _olympusDispatcher = olympusDispatcher;
  private readonly ILogger<TestInteractionModule> _logger = logger;

  [SlashCommand("testinteraction", "Test Olympus")]
  public async Task<string> TestInteraction([SlashCommandParameter(Description = "The text to interact with")] string interactionText)
  {
    //LogTestCommandExecuted(_logger, interactionText);

    // Create TestInteractionCommand
    var command = new TestAiInteractionCommand(interactionText);

    var result = await _olympusDispatcher.DispatchCommandAsync(command);

    return result.IsSuccess ? "Bless" : "Cursed";
  }

  [LoggerMessage(
    Level = LogLevel.Information,
    EventId = 0,
    Message = "Test command executed with text: {InteractionText}")]
  public static partial void LogTestCommandExecuted(ILogger logger, string interactionText);
}
