using Olympus.Application.Common.Types;

namespace Olympus.Application.Common.Messaging;

public interface IOlympusDispatcher
{
  Task<OlympusResult<TResult, OlympusError>> DispatchCommandAsync<TResult>(in IOlympusCommand<TResult> command, CancellationToken cancellationToken = default) where TResult : notnull;

  Task<OlympusResult<TResult, OlympusError>> DispatchQueryAsync<TResult>(in IOlympusQuery<TResult> query, CancellationToken cancellationToken = default) where TResult : notnull;
}
