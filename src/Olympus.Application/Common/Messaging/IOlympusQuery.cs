namespace Olympus.Application.Common.Messaging;

/// <summary>
/// Marker interface for queries that retrieve data in the Olympus application
/// </summary>
/// <typeparam name="TResult">The type of result retrieved by this query</typeparam>
public interface IOlympusQuery<out TResult> { }
