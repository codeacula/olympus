using Olympus.Application.Ai;
using Olympus.Application.AiDrivenFeatures.Common.DTOs;
using Olympus.Application.Common.Types;

namespace Olympus.Infrastructure.Ai.KernelServices;

/// <summary>
/// Implements ISemanticKernelOrchestrator using Semantic Kernel.
/// </summary>
public sealed class SemanticKernelOrchestrator : ISemanticKernelOrchestrator
{
  public ValueTask<OlympusResult<NarrativeResponseDto, OlympusError>> GenerateNarrativeAsync(
      string sessionId,
      string playerId,
      string inputText,
      CancellationToken cancellationToken = default)
  {
    // TODO: Implement actual orchestration logic
    return new ValueTask<OlympusResult<NarrativeResponseDto, OlympusError>>(
        OlympusResult<NarrativeResponseDto, OlympusError>.Fail(new OlympusError("NotImplemented")));
  }
}
