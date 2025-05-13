using Olympus.Application.AiDrivenFeatures.Common.DTOs;
using Olympus.Application.Common.Types;

namespace Olympus.Application.Ai;

/// <summary>
/// Orchestrates AI narrative generation using Semantic Kernel.
/// </summary>
public interface ISemanticKernelOrchestrator
{
    ValueTask<Result<NarrativeResponseDto, Error>> GenerateNarrativeAsync(
        string sessionId,
        string playerId,
        string inputText,
        CancellationToken cancellationToken = default);
}
