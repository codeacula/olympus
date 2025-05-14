using Microsoft.Extensions.Logging;
using Olympus.Application.Ai;
using Olympus.Application.AiDrivenFeatures.Common.DTOs;
using Olympus.Application.Common.Messaging;
using Olympus.Application.Common.Types;

namespace Olympus.Application.AiDrivenFeatures.ProcessPlayerNarrativeInput;

/// <summary>
/// Handles processing a player's narrative input and generating an AI response.
/// </summary>
/// <param name="orchestrator"></param>
/// <param name="contextService"></param>
/// <param name="logger"></param>
public sealed class ProcessPlayerNarrativeInputCommandHandler(
    ISemanticKernelOrchestrator orchestrator,
    IGameSessionNarrativeContextService contextService,
    ILogger<ProcessPlayerNarrativeInputCommandHandler> logger) :
    IOlympusCommandHandler<ProcessPlayerNarrativeInputCommand, Result<NarrativeResponseDto, Error>>
{
  private readonly IGameSessionNarrativeContextService _contextService = contextService;
  private readonly ILogger<ProcessPlayerNarrativeInputCommandHandler> _logger = logger;

  public async Task<Result<NarrativeResponseDto, Error>> HandleAsync(
      ProcessPlayerNarrativeInputCommand command,
      CancellationToken cancellationToken)
  {
    using var _ = _logger.BeginScope("Processing narrative input for session {SessionId}", command.SessionId);
    _logger.LogInformation("Processing narrative input: {InputText}", command.Input);

    try
    {
      // Get the current context (or create new one if it doesn't exist)
      var contextOption = await _contextService.GetContextAsync(
          command.SessionId.Value,
          cancellationToken);

      // For the MVP, we'll generate a simple response
      // In a real implementation, this would use the semantic kernel to process the input
      // based on the current context and generate a response
      var aiResponse = "This is a simulated AI response for the MVP. The AI acknowledges your input: " + command.Input;

      // Create a list of updated context elements (in a real implementation, this would be more sophisticated)
      var updatedContext = new List<string>
      {
          $"User {command.UserId.Value} said: {command.Input}",
          $"AI responded: {aiResponse}"
      };

      // Return the response DTO
      return Result<NarrativeResponseDto, Error>.Ok(new NarrativeResponseDto(
          aiResponse,
          updatedContext));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error processing narrative input");
      return Result<NarrativeResponseDto, Error>.Fail(new Error("ProcessingError", ex.Message));
    }
  }
}
