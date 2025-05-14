namespace Olympus.Domain.SharedKernel.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a session.
/// </summary>
/// <param name="Value"></param>
public sealed record class SessionId(string Value);
