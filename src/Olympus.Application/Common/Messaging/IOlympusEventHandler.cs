namespace Olympus.Application.Common.Messaging;

/// <summary>
/// Handler interface for processing Olympus domain events
/// </summary>
/// <typeparam name="TEvent">Type of event to handle</typeparam>
public interface IOlympusEventHandler<in TEvent>
    where TEvent : IOlympusEvent
{
  /// <summary>
  /// Handles the specified event
  /// </summary>
  /// <param name="incomingEvent">The event to process</param>
  /// <param name="cancellationToken">Cancellation token</param>
  Task HandleAsync(TEvent incomingEvent, CancellationToken cancellationToken);
}
