namespace Olympus.Infrastructure.Messaging.MediatR.Adapters;

/// <summary>
/// Adapter to convert Olympus commands to MediatR requests
/// </summary>
/// <typeparam name="TCommand">The Olympus command type</typeparam>
/// <typeparam name="TResult">The result type</typeparam>
/// <remarks>
/// Creates a new adapter wrapping the specified command
/// </remarks>
/// <param name="command">The command to wrap</param>
internal sealed class OlympusCommandToMediatRRequest<TCommand, TResult>(TCommand command) : IRequest<TResult>
    where TCommand : IOlympusCommand<TResult>
{
  /// <summary>
  /// The wrapped Olympus command
  /// </summary>
  public TCommand Command { get; } = command;
}
