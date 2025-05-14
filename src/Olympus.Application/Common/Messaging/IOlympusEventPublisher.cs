namespace Olympus.Application.Common.Messaging;

/// <summary>
/// Publisher responsible for publishing domain events in the Olympus application
/// </summary>
public interface IOlympusEventPublisher
{
  /// <summary>
  /// Publishes a domain event to all registered handlers
  /// </summary>
  /// <typeparam name="TEvent">The type of event to publish</typeparam>
  /// <param name="event">The event to publish</param>
  /// <param name="cancellationToken">Cancellation token</param>
  Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
      where TEvent : IOlympusEvent;
}
