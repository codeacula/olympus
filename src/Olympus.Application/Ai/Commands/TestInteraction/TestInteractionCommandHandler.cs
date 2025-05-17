using Olympus.Application.Ai.Requests;
using Olympus.Application.Ai.Services.AiInteractionService;
using Olympus.Application.Common.Messaging;
using Olympus.Application.Common.Types;

namespace Olympus.Application.Ai.Commands.TestInteraction;

public class TestInteractionCommandHandler(IAiInteractionService aiInteractionService) : IOlympusCommandHandler<TestInteractionCommand, TestAiInteractionCommandResult>
{
  public async Task<OlympusResult<TestAiInteractionCommandResult, OlympusError>> HandleAsync(
      TestInteractionCommand command,
      CancellationToken cancellationToken)
  {
    var aiRequest = new TalkWithGmRequest(command.InteractionText);

    var aiResponse = await aiInteractionService.SendAiRequestAsync(aiRequest, cancellationToken);

    return aiResponse is null
      ? new OlympusResult<TestAiInteractionCommandResult, OlympusError>.Failure(
          new OlympusError("Failed to get a valid AI response."))
      : (OlympusResult<TestAiInteractionCommandResult, OlympusError>)new OlympusResult<TestAiInteractionCommandResult, OlympusError>.Success(
        new TestAiInteractionCommandResult(ReplayTest: aiResponse));
  }
}
