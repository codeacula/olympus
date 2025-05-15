using NetCord.Services.Commands;

namespace Olympus.Bot.Discord.Commands;

public class TestInteractionCommand : CommandModule<CommandContext>
{
  [Command("test-interaction")]
  public static string TestCommand([CommandParameter(Remainder = true)] string interactionText)
  {
    return $"Test command executed with text: {interactionText}";
  }
}
