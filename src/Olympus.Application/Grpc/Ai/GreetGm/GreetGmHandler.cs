using Microsoft.Extensions.Logging;
using Olympus.Application.Ai.Interfaces;

namespace Olympus.Application.Grpc.Ai.GreetGm;

internal sealed partial class GreetGmHandler(ILogger<GreetGmHandler> logger, ITheOrb theOrb) : IRequestHandler<GreetGmRequest, GreetGmResponse>
{
  private readonly ILogger<GreetGmHandler> _logger = logger;
  private readonly ITheOrb _theOrb = theOrb;

  public async Task<GreetGmResponse> Handle(GreetGmRequest request, CancellationToken cancellationToken)
  {
    ProcessingRequest(_logger, request.InteractionText.Length);

    try
    {
      var response = await _theOrb.GreetGmAsync(new(request.InteractionText));
      return string.IsNullOrEmpty(response.Response)
        ? throw new OlympusInvalidResponseException("The response from the AI is empty or null.")
        : new GreetGmResponse(new(response.Response));
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
