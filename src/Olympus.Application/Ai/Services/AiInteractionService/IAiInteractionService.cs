namespace Olympus.Application.Ai.Services.AiInteractionService;

public interface IAiInteractionService
{
  Task<IAiResponse> SendAiRequestAsync(IAiRequest request, CancellationToken? cancellationToken = null);
}
