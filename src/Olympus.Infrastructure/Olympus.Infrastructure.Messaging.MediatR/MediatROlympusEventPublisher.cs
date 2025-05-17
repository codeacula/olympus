namespace Olympus.Infrastructure.Messaging.MediatR;

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
