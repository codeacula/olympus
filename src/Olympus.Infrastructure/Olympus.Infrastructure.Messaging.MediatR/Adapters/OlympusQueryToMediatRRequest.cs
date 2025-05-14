using MediatR;
using Olympus.Application.Common.Messaging;

namespace Olympus.Infrastructure.Messaging.MediatR.Adapters;

/// <summary>
/// Adapter to convert Olympus queries to MediatR requests
/// </summary>
/// <typeparam name="TQuery">The Olympus query type</typeparam>
/// <typeparam name="TResult">The result type</typeparam>
/// <remarks>
/// Creates a new adapter wrapping the specified query
/// </remarks>
/// <param name="query">The query to wrap</param>
internal class OlympusQueryToMediatRRequest<TQuery, TResult>(TQuery query) : IRequest<TResult>
    where TQuery : IOlympusQuery<TResult>
{
  /// <summary>
  /// The wrapped Olympus query
  /// </summary>
  public TQuery Query { get; } = query;
}
