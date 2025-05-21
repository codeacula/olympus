using Olympus.Application.Ai.Interfaces;
using Olympus.Application.Ai.VOs;

namespace Olympus.Ai;

public class TheOrb : ITheOrb
{
  public Task<AiResponse> GreetGmAsync(UserPrompt userPrompt)
  {
    // TODO: Implement actual greeting logic
    return Task.FromResult(new($"Hello, GM! You said: {userPrompt.Prompt}"));
  }
}
