using System.Reflection;
using MediatR;
using Olympus.Application.Common.Messaging;
using Olympus.Infrastructure.Messaging.MediatR.Adapters;

namespace Olympus.Infrastructure.Messaging.MediatR;

/// <summary>
/// Implementation of the Olympus dispatcher using MediatR
/// </summary>
public class MediatROlympusDispatcher : IOlympusDispatcher
{
  private readonly IMediator _mediator;

  public MediatROlympusDispatcher(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<TResult> DispatchCommandAsync<TResult>(
      IOlympusCommand<TResult> command,
      CancellationToken cancellationToken = default)
  {
    var commandType = command.GetType();
    var requestType = typeof(OlympusCommandToMediatRRequest<,>).MakeGenericType(commandType, typeof(TResult));
    var request = Activator.CreateInstance(requestType, command);

    return await _mediator.Send(request, cancellationToken);
  }

  public async Task<TResult> DispatchQueryAsync<TResult>(
      IOlympusQuery<TResult> query,
      CancellationToken cancellationToken = default)
  {
    var queryType = query.GetType();
    var requestType = typeof(OlympusQueryToMediatRRequest<,>).MakeGenericType(queryType, typeof(TResult));
    var request = Activator.CreateInstance(requestType, query);

    return await _mediator.Send(request, cancellationToken);
  }
}
