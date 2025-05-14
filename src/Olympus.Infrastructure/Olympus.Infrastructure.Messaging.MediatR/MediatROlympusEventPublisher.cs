using MediatR;
using Olympus.Application.Common.Messaging;
using Olympus.Infrastructure.Messaging.MediatR.Adapters;

namespace Olympus.Infrastructure.Messaging.MediatR;

/// <summary>
/// Implementation of the Olympus event publisher using MediatR
/// </summary>
/// <param name="publisher"></param>
public class MediatROlympusEventPublisher(IPublisher publisher) : IOlympusEventPublisher
{
  private readonly IPublisher _publisher = publisher;

  public async Task PublishAsync<TEvent>(
      TEvent @event,
      CancellationToken cancellationToken = default)
      where TEvent : IOlympusEvent
  {
    var notification = new OlympusEventToMediatRNotification<TEvent>(@event);
    await _publisher.Publish(notification, cancellationToken);
  }
}
