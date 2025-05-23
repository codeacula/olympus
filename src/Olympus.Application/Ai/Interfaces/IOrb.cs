using Microsoft.SemanticKernel.ChatCompletion;

namespace Olympus.Application.Ai.Interfaces;

public interface IOrb
{
  ChatHistory ChatHistory { get; }
  Task<AiResponse> GreetGmAsync(UserPrompt userPrompt);
}
