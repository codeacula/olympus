using NetCord.Services.ApplicationCommands;

namespace Olympus.Bot.Discord.Commands;

public class TestInteractionModule(ILogger<TestInteractionModule> logger) : ApplicationCommandModule<ApplicationCommandContext>
{
  private readonly ILogger<TestInteractionModule> _logger = logger;

  [SlashCommand("testinteraction", "Test Olympus")]
  public string TestCommand([SlashCommandParameter(Description = "The text to interact with")] string interactionText)
  {
    _logger.LogInformation("Test command executed with text: {InteractionText}", interactionText);
    return $"Test command executed with text: {interactionText}";
  }
}
