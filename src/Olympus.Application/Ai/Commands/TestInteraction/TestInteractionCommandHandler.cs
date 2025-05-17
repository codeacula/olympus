using Olympus.Application.Ai.Errors;
using Olympus.Application.Ai.Interactions.TalkWithGm;
using Olympus.Application.Common.Messaging;
using Olympus.Application.Common.Types;
using Olympus.Application.Grpc;

namespace Olympus.Application.Ai.Commands.TestInteraction;

public class TestInteractionCommandHandler(IGrpcClient grpcClient) : IOlympusCommandHandler<TestInteractionCommand, TestAiInteractionCommandResult>
{
  public async Task<OlympusResult<TestAiInteractionCommandResult, OlympusError>> HandleAsync(
      TestInteractionCommand command,
      CancellationToken cancellationToken)
  {
    var aiRequest = new TalkWithGmRequest(command.InteractionText);

    var aiResponse = await grpcClient.AiApiService.TalkWithGmAsync(aiRequest, cancellationToken);

    return aiResponse is null
      ? new OlympusResult<TestAiInteractionCommandResult, OlympusError>.Failure(
          new FailedToGetResponseError("Failed to get a valid AI response."))
      : new OlympusResult<TestAiInteractionCommandResult, OlympusError>.Success(
        new TestAiInteractionCommandResult(ReplayTest: aiResponse.ToString()!));
  }
}
