using Microsoft.SemanticKernel.ChatCompletion;

namespace Olympus.Application.Ai;

public interface IOrb
{
  ChatHistory ChatHistory { get; }
  Task<AiResponse> GreetGmAsync(UserPrompt userPrompt);
}
