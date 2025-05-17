namespace Olympus.Application.Ai.Commands.TestInteraction;

public class TestInteractionCommandHandler(IAiInteractionService aiInteractionService) : IOlympusCommandHandler<TestInteractionCommand, TestAiInteractionCommandResult>
{
  public async Task<OlympusResult<TestAiInteractionCommandResult, OlympusError>> HandleAsync(
      TestInteractionCommand command,
      CancellationToken cancellationToken)
  {
    var aiRequest = new TalkWithGmRequest(command.InteractionText);
    return await aiInteractionService.SendAiRequestAsync(aiRequest, cancellationToken);
  }
}
