using MediatR;
using Olympus.Application.Common.Messaging;

namespace Olympus.Infrastructure.Messaging.MediatR.Adapters;

/// <summary>
/// Adapter to convert Olympus commands to MediatR requests
/// </summary>
/// <typeparam name="TCommand">The Olympus command type</typeparam>
/// <typeparam name="TResult">The result type</typeparam>
internal class OlympusCommandToMediatRRequest<TCommand, TResult> : IRequest<TResult>
    where TCommand : IOlympusCommand<TResult>
{
    /// <summary>
    /// The wrapped Olympus command
    /// </summary>
    public TCommand Command { get; }

    /// <summary>
    /// Creates a new adapter wrapping the specified command
    /// </summary>
    /// <param name="command">The command to wrap</param>
    public OlympusCommandToMediatRRequest(TCommand command)
    {
        Command = command;
    }
}
