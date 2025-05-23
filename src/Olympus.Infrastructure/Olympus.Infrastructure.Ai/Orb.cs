using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Olympus.Application.Ai;

namespace Olympus.Infrastructure.Ai;

public partial class Orb(AiBrain aiBrain, ILogger<Orb> logger) : IOrb
{
  private readonly AiBrain _aiBrain = aiBrain;
  private readonly ILogger<Orb> _logger = logger;
  public ChatHistory ChatHistory { get; } = [];

  public async Task<AiResponse> GreetGmAsync(UserPrompt userPrompt)
  {
    LogExecutingAction(_logger, nameof(GreetGmAsync), userPrompt.Prompt);
    var request = new ChatMessageRequest(userPrompt.Prompt);
    var response = await _aiBrain.GetChatMessageAsync(this, request);
    return new(response.Response);
  }

  [LoggerMessage(Level = LogLevel.Information, Message = "Executing action {ActionName} with prompt {Prompt}")]
  private static partial void LogExecutingAction(ILogger<Orb> logger, string actionName, string prompt);
}
