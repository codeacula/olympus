using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Olympus.Application.Ai.Interfaces;
using Olympus.Application.Common.Exceptions;

namespace Olympus.Ai.Infrastructure;

internal sealed partial class SemanticKernelOrchestrator(Kernel kernel, ILogger<SemanticKernelOrchestrator> logger) : ISemanticKernelOrchestrator
{
  private readonly Kernel _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
  private readonly ILogger<SemanticKernelOrchestrator> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

  public async Task<string> ExecutePromptAsync(string promptName, object variables)
  {
    try
    {
      // Configure the kernel for this execution
      var kernelArguments = new KernelArguments();

      // Add variables to kernel arguments
      foreach (var prop in variables.GetType().GetProperties())
      {
        kernelArguments[prop.Name] = prop.GetValue(variables);
      }

      // Execute the prompt
      var result = await _kernel.InvokePromptAsync(promptName, kernelArguments);
      return result.GetValue<string>() ?? throw new OlympusInvalidResponseException("The response from the AI is empty or null.");
    }
    catch (Exception ex)
    {
      LogErrorExecutingPrompt(_logger, promptName, ex);
      throw new OlympusAiException($"Failed to execute prompt {promptName}", ex);
    }
  }

  public Task<string> GreetGmAsync(string interactionText)
  {
    throw new NotImplementedException();
  }

  [LoggerMessage(Level = LogLevel.Error, Message = "Error executing prompt {PromptName}")]
  private static partial void LogErrorExecutingPrompt(ILogger logger, string promptName, Exception ex);
}
