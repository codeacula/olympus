using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Olympus.Application.Ai;
using Olympus.Application.Common.Exceptions;

namespace Olympus.Infrastructure.Ai;

public sealed partial class AiBrain : IAiBrain
{
  private readonly AiConfig _aiConfig;
  private readonly IChatCompletionService _chatCompletionService;
  private readonly Kernel _kernel;
  private readonly ILogger<AiBrain> _logger;
  private readonly OpenAIPromptExecutionSettings _promptExecutionSettings = new()
  {
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
  };

  public AiBrain(AiConfig aiConfig, ILogger<AiBrain> logger)
  {
    _aiConfig = aiConfig ?? throw new ArgumentNullException(nameof(aiConfig));
    _logger = logger;

    _kernel = GetBuilder().Build();

    _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
  }

  public async Task<ChatMessageResponse> GetChatMessageAsync(IOrb speakingOrb, ChatMessageRequest request)
  {
    try
    {
      speakingOrb.ChatHistory.AddUserMessage(request.Prompt);
      // Execute the prompt
      var result = await _chatCompletionService.GetChatMessageContentAsync(
        speakingOrb.ChatHistory,
        _promptExecutionSettings,
        _kernel
      ) ?? throw new OlympusInvalidResponseException("The response from the AI is empty or null.");

      speakingOrb.ChatHistory.AddMessage(result.Role, result.ToString());

      return new ChatMessageResponse(result.ToString());
    }
    catch (Exception ex) when (ex is not OlympusInvalidResponseException)
    {
      LogErrorExecutingPrompt(_logger, request.Prompt, ex);
      throw new OlympusAiException($"Failed to execute prompt {request.Prompt}", ex);
    }
  }

  private IKernelBuilder GetBuilder()
  {
    var builder = Kernel.CreateBuilder();
    _ = builder.AddOpenAIChatCompletion(_aiConfig.ModelId, _aiConfig.Endpoint, _aiConfig.ApiKey);

    _ = builder.Services.AddLogging(services => services.AddConsole());

    return builder;
  }

  [LoggerMessage(Level = LogLevel.Error, Message = "Error executing prompt {PromptName}")]
  private static partial void LogErrorExecutingPrompt(ILogger logger, string promptName, Exception ex);
}
