namespace Olympus.Application.Common.Messaging;

public interface IOlympusDispatcher
{
  Task<OlympusResult<TResult>> DispatchCommandAsync<TResult>(in IOlympusCommand command, CancellationToken cancellationToken = default);

  Task<OlympusResult<TResult>> DispatchQueryAsync<TResult>(in IOlympusQuery<TResult> query, CancellationToken cancellationToken = default);
}
