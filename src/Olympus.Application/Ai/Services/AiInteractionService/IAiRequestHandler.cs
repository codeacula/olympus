namespace Olympus.Application.Ai.Services.AiInteractionService;

public interface IAiRequestHandler<in TRequest, TResponse>
    where TRequest : IAiRequest<TResponse>
    where TResponse : IAiResponse
{
  Task<TResponse> HandleRequestAsync(TRequest request, CancellationToken? cancellationToken = null);
}
