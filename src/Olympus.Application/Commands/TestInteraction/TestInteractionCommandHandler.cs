using Olympus.Application.Grpc;
using Olympus.Application.Grpc.Ai.TalkWithGm;

namespace Olympus.Application.Commands.TestInteraction;

internal sealed class TestInteractionCommandHandler(IGrpcClient grpcClient) : IRequestHandler<TestInteractionCommand, TestInteractionResult>
{
  public async Task<TestInteractionResult> Handle(TestInteractionCommand request, CancellationToken cancellationToken)
  {
    var aiRequest = new TalkWithGmRequest(request.InteractionText);

    var aiResponse = await grpcClient.AiApiService.TalkWithGmAsync(aiRequest, cancellationToken) ??
      throw new OlympusInvalidResponseException("AI response is null.");

    return new TestInteractionResult(Message: aiResponse.ToString()!);
  }
}
