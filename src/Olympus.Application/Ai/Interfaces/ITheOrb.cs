using Olympus.Application.Ai.VOs;

namespace Olympus.Application.Ai.Interfaces;

public interface ITheOrb
{
  Task<AiResponse> GreetGmAsync(UserPrompt userPrompt);
}
