using Olympus.Application.Common.Types;

namespace Olympus.Application.Ai.Services.AiInteractionService;

public interface IAiInteractionService
{
  Task<OlympusResult<TResponseType, TErrorType>> SendAiRequestAsync<TResponseType, TErrorType>(
      IAiRequest<TResponseType> request,
      CancellationToken? cancellationToken = null)
    where TResponseType : IAiResponse
    where TErrorType : OlympusError;
}
