using Olympus.Application.Ai.Interfaces;

namespace Olympus.Ai;

public class TheOrb : ITheOrb
{
  public Task<string> GreetGmAsync(string interactionText)
  {
    // TODO: Implement actual greeting logic
    return Task.FromResult($"Hello, GM! You said: {interactionText}");
  }
}
