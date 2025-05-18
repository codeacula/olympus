using MediatR;

namespace Olympus.Domain.Common.Events;

/// <summary>
/// Base class for all domain events in the Olympus system.
/// </summary>
public abstract record class DomainEventBase : INotification
{
  /// <summary>
  /// The UTC timestamp when the event was created.
  /// </summary>
  public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}
