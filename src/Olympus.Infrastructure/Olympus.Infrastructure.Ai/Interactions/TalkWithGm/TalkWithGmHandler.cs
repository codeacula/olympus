using Microsoft.Extensions.Logging;
using Olympus.Application.Ai.Errors;
using Olympus.Application.Ai.Interactions.TalkWithGm;
using Olympus.Application.Ai.Services.AiInteractionService;
using Olympus.Application.Common.Types;

namespace Olympus.Infrastructure.Ai.Handlers;

internal sealed partial class TalkWithGmHandler(ILogger<TalkWithGmHandler> logger) : IAiRequestHandler<TalkWithGmRequest, TalkWithGmResponse, FailedToGetResponseError>
{
  private readonly ILogger<TalkWithGmHandler> _logger = logger;
  public async Task<OlympusResult<TalkWithGmResponse, FailedToGetResponseError>> HandleRequestAsync(
      TalkWithGmRequest request,
      CancellationToken? cancellationToken = null)
  {
    ProcessingGmRequest(_logger, request.Message.Length);

    var token = cancellationToken ?? CancellationToken.None;

    try
    {
      // In a real implementation, you would use Semantic Kernel to process the request
      // This is a placeholder implementation
      var response = $"GM: I heard you say \"{request.Message}\". How can I help you with your adventure?";

      // Add a small delay to make this method truly async for demonstration purposes
      await Task.Delay(1, token);

      return new OlympusResult<TalkWithGmResponse, FailedToGetResponseError>.Success(new TalkWithGmResponse(response));
    }
    catch (Exception ex)
    {
      ErrorProcessingGmRequest(_logger, ex);
      return new OlympusResult<TalkWithGmResponse, FailedToGetResponseError>.Failure(new FailedToGetResponseError("I apologize, but I'm having trouble responding right now."));
    }
  }

  [LoggerMessage(Level = LogLevel.Information, Message = "Processing GM conversation request: {MessageLength} chars")]
  public static partial void ProcessingGmRequest(ILogger logger, int messageLength);

  [LoggerMessage(Level = LogLevel.Error, Message = "Error processing GM conversation request")]
  public static partial void ErrorProcessingGmRequest(ILogger logger, Exception exception);
}
