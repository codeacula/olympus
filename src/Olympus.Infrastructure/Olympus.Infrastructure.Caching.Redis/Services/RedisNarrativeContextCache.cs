using Olympus.Application.Ai;
using Olympus.Application.AiDrivenFeatures.Common.DTOs;
using Olympus.Application.Common.Types;

namespace Olympus.Infrastructure.Caching.Redis.Services;

/// <summary>
/// Implements IGameSessionNarrativeContextService using Redis for storage.
/// </summary>
public sealed class RedisNarrativeContextCache : IGameSessionNarrativeContextService
{
  public ValueTask<OlympusOption<NarrativeContext>> GetContextAsync(string sessionId, CancellationToken cancellationToken = default)
  {
    // TODO: Implement Redis fetch logic
    return new ValueTask<OlympusOption<NarrativeContext>>(OlympusOption<NarrativeContext>.NoValue());
  }

  public ValueTask SaveContextAsync(NarrativeContext context, CancellationToken cancellationToken = default)
  {
    // TODO: Implement Redis save logic
    return ValueTask.CompletedTask;
  }
}
