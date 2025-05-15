namespace Olympus.Application.Common.Messaging;

/// <summary>
/// Central dispatcher responsible for sending commands and queries in the Olympus application
/// </summary>
public interface IOlympusDispatcher
{
  /// <summary>
  /// Dispatches a command to its handler and returns the result
  /// </summary>
  /// <typeparam name="TResult">The type of result expected from the command</typeparam>
  /// <param name="command">The command to dispatch</param>
  /// <param name="cancellationToken">Casncellation token</param>
  /// <returns>The result from the command handler</returns>
  Task<TResult> DispatchCommandAsync<TResult>(IOlympusCommand<TResult> command, CancellationToken cancellationToken = default);

  /// <summary>
  /// Dispatches a query to its handler and returns the result
  /// </summary>
  /// <typeparam name="TResult">The type of result expected from the query</typeparam>
  /// <param name="query">The query to dispatch</param>
  /// <param name="cancellationToken">Cancellation token</param>
  /// <returns>The result from the query handler</returns>
  Task<TResult> DispatchQueryAsync<TResult>(IOlympusQuery<TResult> query, CancellationToken cancellationToken = default);
}
