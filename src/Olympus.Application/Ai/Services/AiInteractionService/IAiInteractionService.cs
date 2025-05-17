namespace Olympus.Application.Ai.Services.AiInteractionService;

public interface IAiInteractionService
{
  Task<TResponseType> SendAiRequestAsync<TResponseType>(
      IAiRequest<TResponseType> request,
      CancellationToken? cancellationToken = null)
    where TResponseType : IAiResponse;
}
