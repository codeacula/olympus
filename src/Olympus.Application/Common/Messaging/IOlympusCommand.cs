namespace Olympus.Application.Common.Messaging;

/// <summary>
/// Marker interface for commands that produce a result in the Olympus application
/// </summary>
/// <typeparam name="TResult">The type of result produced by this command</typeparam>
public interface IOlympusCommand<out TResult> { }
