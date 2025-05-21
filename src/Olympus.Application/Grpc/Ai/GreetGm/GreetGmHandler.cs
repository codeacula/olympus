using Microsoft.Extensions.Logging;

namespace Olympus.Application.Grpc.Ai.GreetGm;

internal sealed partial class GreetGmHandler(ILogger<GreetGmHandler> logger) : IRequestHandler<GreetGmRequest, GreetGmResponse>
{
  private readonly ILogger<GreetGmHandler> _logger = logger;
  public async Task<GreetGmResponse> Handle(GreetGmRequest request, CancellationToken cancellationToken)
  {
    ProcessingRequest(_logger, request.InteractionText.Length);

    try
    {
      // TODO: Implement actual greeting logic
      var response = $"Hello, GM! You said: {request.InteractionText}";
      return new GreetGmResponse(response);
    }
    catch (Exception ex)
    {
      ErrorProcessingRequest(_logger, ex);
      throw new OlympusInvalidResponseException("An error occurred while processing the request.", ex);
    }
  }

  [LoggerMessage(Level = LogLevel.Information, Message = "Processing conversation request: {MessageLength} chars")]
  public static partial void ProcessingRequest(ILogger logger, int messageLength);

  [LoggerMessage(Level = LogLevel.Error, Message = "Error processing conversation request")]
  public static partial void ErrorProcessingRequest(ILogger logger, Exception exception);
}
