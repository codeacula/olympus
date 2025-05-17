using Grpc.Core;

namespace Olympus.Api.Services;

public class AiGrpcService(ILogger<AiGrpcService> logger) : AiInteraction.AiInteractionBase
{
  private readonly ILogger<AiGrpcService> _logger = logger;

  public override async Task<NarrativeResponseProto> Interact(PlayerInteractRequestProto request, ServerCallContext context)
  {
    _logger.LogInformation("gRPC Interact called for user {UserId}", request.UserId);

    // 1. Map Proto request to Application Command (if using CQRS)
    // var command = new Application.AiDrivenFeatures.ProcessPlayerNarrativeInput.ProcessPlayerNarrativeInputCommand(
    //     request.SessionId,
    //     request.UserId,
    //     request.Input);
    // var result = await _dispatcher.Send(command);

    // For now, mock response similar to your HTTP controller:
    // Replace with actual call to Application layer
    var mockAppResponse = new NarrativeResponseDto(
        $"Mock gRPC response to: {request.Input}",
        ["gRPC Context item 1", "gRPC Context item 2"]
    );

    // 2. Map Application DTO/Result to Proto response
    var responseProto = new NarrativeResponseProto
    {
      ResponseText = mockAppResponse.Narrative,
    };
    responseProto.ContextItems.AddRange(mockAppResponse.ContextItems);

    return responseProto; // await Task.FromResult(responseProto) if mapping is async
  }
}
