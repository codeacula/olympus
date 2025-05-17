using Olympus.Application.Common.Messaging;

namespace Olympus.Application.Ai.Commands.TestInteractionCommand;

public class TestAiInteractionCommand(string interactionText) : IOlympusCommand<string>
{
  public string InteractionText { get; } = interactionText;
}
