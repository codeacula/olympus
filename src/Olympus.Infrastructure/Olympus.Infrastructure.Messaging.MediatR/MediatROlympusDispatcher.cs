using MediatR;
using Olympus.Application.Common.Messaging;
using Olympus.Infrastructure.Messaging.MediatR.Adapters;

namespace Olympus.Infrastructure.Messaging.MediatR;

/// <summary>
/// Implementation of the Olympus dispatcher using MediatR
/// </summary>
/// <param name="mediator"></param>
public class MediatROlympusDispatcher(IMediator mediator) : IOlympusDispatcher
{
  private readonly IMediator _mediator = mediator;

  public async Task<TResult> DispatchCommandAsync<TResult>(
      IOlympusCommand<TResult> command,
      CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    var commandType = command.GetType();
    var requestType = typeof(OlympusCommandToMediatRRequest<,>).MakeGenericType(commandType, typeof(TResult));
    var request = Activator.CreateInstance(requestType, command)
      ?? throw new InvalidOperationException($"Failed to create request of type {requestType.Name}");

    var result = await _mediator.Send(request, cancellationToken);
    return result is TResult typedResult
      ? typedResult
      : throw new InvalidOperationException($"Expected result of type {typeof(TResult).Name} but got {result?.GetType().Name ?? "null"}");
  }

  public async Task<TResult> DispatchQueryAsync<TResult>(
      IOlympusQuery<TResult> query,
      CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(query);
    var queryType = query.GetType();
    var requestType = typeof(OlympusQueryToMediatRRequest<,>).MakeGenericType(queryType, typeof(TResult));
    var request = Activator.CreateInstance(requestType, query)
      ?? throw new InvalidOperationException($"Failed to create request of type {requestType.Name}");

    var result = await _mediator.Send(request, cancellationToken);
    return result is TResult typedResult
      ? typedResult
      : throw new InvalidOperationException($"Expected result of type {typeof(TResult).Name} but got {result?.GetType().Name ?? "null"}");
  }
}
