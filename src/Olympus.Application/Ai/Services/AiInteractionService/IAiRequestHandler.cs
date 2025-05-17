using Olympus.Application.Common.Types;

namespace Olympus.Application.Ai.Services.AiInteractionService;

public interface IAiRequestHandler<in TRequest, TResponse, TError>
    where TRequest : IAiRequest<TResponse>
    where TResponse : IAiResponse
    where TError : OlympusError
{
  Task<OlympusResult<TResponse, TError>> HandleRequestAsync(TRequest request, CancellationToken? cancellationToken = null);
}
