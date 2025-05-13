using Olympus.Application.AiDrivenFeatures.Common.DTOs;
using Olympus.Application.Common.Types;

namespace Olympus.Application.Ai;

/// <summary>
/// Provides access to narrative context for a game session.
/// </summary>
public interface IGameSessionNarrativeContextService
{
    ValueTask<Option<NarrativeContext>> GetContextAsync(string sessionId, CancellationToken cancellationToken = default);
    ValueTask SaveContextAsync(NarrativeContext context, CancellationToken cancellationToken = default);
}
