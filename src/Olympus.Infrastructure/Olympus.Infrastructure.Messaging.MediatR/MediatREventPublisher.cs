using Olympus.Application.Abstractions;

namespace Olympus.Infrastructure.Messaging.MediatR;

/// <summary>
/// Publishes events using MediatR.
/// </summary>
public sealed class MediatREventPublisher : IEventPublisher
{
  public ValueTask PublishAsync<TEvent>(TEvent incomingEvent, CancellationToken cancellationToken = default)
  {
    // TODO: Implement MediatR publish logic
    return ValueTask.CompletedTask;
  }
}
