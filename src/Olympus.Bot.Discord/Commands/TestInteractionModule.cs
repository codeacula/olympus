using NetCord.Services.ApplicationCommands;
using Olympus.Application.Ai.Commands.TestInteractionCommand;

namespace Olympus.Bot.Discord.Commands;

public partial class TestInteractionModule(ILogger<TestInteractionModule> logger) : ApplicationCommandModule<ApplicationCommandContext>
{
  private readonly ILogger<TestInteractionModule> _logger = logger;

  [SlashCommand("testinteraction", "Test Olympus")]
  public string TestInteraction([SlashCommandParameter(Description = "The text to interact with")] string interactionText)
  {
    //LogTestCommandExecuted(_logger, interactionText);

    // Create TestInteractionCommand
    var command = new TestAiInteractionCommand(interactionText);
    // Send the command to the service bus
    return "Bless";
  }

  [LoggerMessage(
    Level = LogLevel.Information,
    EventId = 0,
    Message = "Test command executed with text: {InteractionText}")]
  public static partial void LogTestCommandExecuted(ILogger logger, string interactionText);
}
