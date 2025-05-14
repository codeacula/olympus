namespace Olympus.Application.Common.Messaging;

/// <summary>
/// Handler interface for processing Olympus queries
/// </summary>
/// <typeparam name="TQuery">Type of query to handle</typeparam>
/// <typeparam name="TResult">Type of result produced by the query</typeparam>
public interface IOlympusQueryHandler<in TQuery, TResult>
    where TQuery : IOlympusQuery<TResult>
{
  /// <summary>
  /// Handles the specified query and returns a result
  /// </summary>
  /// <param name="query">The query to process</param>
  /// <param name="cancellationToken">Cancellation token</param>
  /// <returns>The result of processing the query</returns>
  Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
