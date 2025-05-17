namespace Olympus.Application.Common.Messaging;

public interface IOlympusEventPublisher
{
  Task PublishAsync<TEvent>(TEvent incomingEvent, CancellationToken cancellationToken = default)
      where TEvent : IOlympusEvent;
}
