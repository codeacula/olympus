using Olympus.Application.Common.Types;

namespace Olympus.Application.Common.Messaging;

public interface IOlympusCommandHandler<in TCommand, TResult>
    where TCommand : IOlympusCommand<TResult>
    where TResult : notnull
{
  Task<OlympusResult<TResult, OlympusError>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
