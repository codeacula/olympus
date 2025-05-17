using Olympus.Application.Ai.Services.AiInteractionService;
using Olympus.Application.Common.Types;

namespace Olympus.Infrastructure.Ai.Services;

public class AiInteractionService : IAiInteractionService
{
  private readonly IServiceProvider _serviceProvider;

  public AiInteractionService(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public async Task<OlympusResult<TResponseType, TErrorType>> SendAiRequestAsync<TResponseType, TErrorType>(IAiRequest<TResponseType> request, CancellationToken? cancellationToken = null)
      where TResponseType : IAiResponse
      where TErrorType : OlympusError
  {
    // Get the concrete request type and response type
    var requestType = request.GetType();
    var responseType = typeof(TResponseType);

    // Get the handler interface type with the concrete request and response types
    var handlerInterfaceType = typeof(IAiRequestHandler<,,>).MakeGenericType(requestType, responseType);

    // Resolve the handler from the service provider
    var handler = _serviceProvider.GetService(handlerInterfaceType) ??
        throw new InvalidOperationException($"No registered handler found for {requestType.Name} and {responseType.Name}");

    // Cast to the expected handler interface
    return handler is not IAiRequestHandler<IAiRequest<TResponseType>, TResponseType, TErrorType> typedHandler
        ? new OlympusResult<TResponseType, TErrorType>.Failure(default!)
        : await typedHandler.HandleRequestAsync(request, cancellationToken);
  }
}
