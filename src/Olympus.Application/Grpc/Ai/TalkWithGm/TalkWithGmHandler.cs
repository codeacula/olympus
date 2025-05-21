using Microsoft.Extensions.Logging;

namespace Olympus.Application.Grpc.Ai.TalkWithGm;

internal sealed partial class TalkWithGmHandler(ILogger<TalkWithGmHandler> logger) : IRequestHandler<TalkWithGmRequest, TalkWithGmResponse>
{
  private readonly ILogger<TalkWithGmHandler> _logger = logger;
  public async Task<TalkWithGmResponse> Handle(TalkWithGmRequest request, CancellationToken cancellationToken)
  {
    ProcessingGmRequest(_logger, request.InteractionText.Length);

    try
    {
      // In a real implementation, you would use Semantic Kernel to process the request
      // This is a placeholder implementation
      var response = $"GM: I heard you say \"{request.InteractionText}\". How can I help you with your adventure?";

      // Add a small delay to make this method truly async for demonstrati  on purposes
      await Task.Delay(1, cancellationToken);

      return new TalkWithGmResponse(response);
    }
    catch (Exception ex)
    {
      ErrorProcessingGmRequest(_logger, ex);
      throw new OlympusInvalidResponseException("An error occurred while processing the GM request.", ex);
    }
  }

  [LoggerMessage(Level = LogLevel.Information, Message = "Processing GM conversation request: {MessageLength} chars")]
  public static partial void ProcessingGmRequest(ILogger logger, int messageLength);

  [LoggerMessage(Level = LogLevel.Error, Message = "Error processing GM conversation request")]
  public static partial void ErrorProcessingGmRequest(ILogger logger, Exception exception);
}
