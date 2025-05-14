using MediatR;
using Olympus.Application.Common.Messaging;

namespace Olympus.Infrastructure.Messaging.MediatR.Adapters;

/// <summary>
/// Adapter to convert Olympus queries to MediatR requests
/// </summary>
/// <typeparam name="TQuery">The Olympus query type</typeparam>
/// <typeparam name="TResult">The result type</typeparam>
internal class OlympusQueryToMediatRRequest<TQuery, TResult> : IRequest<TResult>
    where TQuery : IOlympusQuery<TResult>
{
  /// <summary>
  /// The wrapped Olympus query
  /// </summary>
  public TQuery Query { get; }

  /// <summary>
  /// Creates a new adapter wrapping the specified query
  /// </summary>
  /// <param name="query">The query to wrap</param>
  public OlympusQueryToMediatRRequest(TQuery query)
  {
    Query = query;
  }
}
