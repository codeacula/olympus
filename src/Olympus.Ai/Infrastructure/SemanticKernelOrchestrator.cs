using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Olympus.Ai.Infrastructure;

internal sealed class SemanticKernelOrchestrator : ISemanticKernelOrchestrator
{
  private readonly Kernel _kernel;
  private readonly ILogger<SemanticKernelOrchestrator> _logger;

  public SemanticKernelOrchestrator(Kernel kernel, ILogger<SemanticKernelOrchestrator> logger)
  {
    _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

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
      return result.GetValue<string>();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error executing prompt {PromptName}", promptName);
      throw new OlympusAiException($"Failed to execute prompt {promptName}", ex);
    }
  }

  // Add other methods for different types of semantic kernel interactions
}
