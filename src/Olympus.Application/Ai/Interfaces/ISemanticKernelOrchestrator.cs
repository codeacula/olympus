namespace Olympus.Application.Ai.Interfaces;

public interface ISemanticKernelOrchestrator
{
  Task<string> GreetGmAsync(string interactionText);
}
