using NetCord.Services.ApplicationCommands;

namespace Olympus.Bot.Discord.Commands;

public partial class TestInteractionModule(ILogger<TestInteractionModule> logger) : ApplicationCommandModule<ApplicationCommandContext>
{
  private readonly ILogger<TestInteractionModule> _logger = logger;

  [SlashCommand("testinteraction", "Test Olympus")]
  public string TestInteraction([SlashCommandParameter(Description = "The text to interact with")] string interactionText)
  {
    LogTestCommandExecuted(_logger, interactionText);
    return $"Test command executed with text: {interactionText}";
  }

  [LoggerMessage(
    Level = LogLevel.Information,
    EventId = 0,
    Message = "Test command executed with text: {InteractionText}")]
  public static partial void LogTestCommandExecuted(ILogger logger, string interactionText);
}
