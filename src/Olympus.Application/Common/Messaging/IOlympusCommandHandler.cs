namespace Olympus.Application.Common.Messaging;

/// <summary>
/// Handler interface for processing Olympus commands
/// </summary>
/// <typeparam name="TCommand">Type of command to handle</typeparam>
/// <typeparam name="TResult">Type of result produced by the command</typeparam>
public interface IOlympusCommandHandler<in TCommand, TResult>
    where TCommand : IOlympusCommand<TResult>
{
    /// <summary>
    /// Handles the specified command and returns a result
    /// </summary>
    /// <param name="command">The command to process</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of processing the command</returns>
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
}
