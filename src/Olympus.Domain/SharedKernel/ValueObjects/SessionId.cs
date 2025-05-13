namespace Olympus.Domain.SharedKernel.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a session.
/// </summary>
public sealed record class SessionId(string Value);
