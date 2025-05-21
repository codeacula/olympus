namespace Olympus.Domain.SharedKernel.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a user.
/// </summary>
/// <param name="Value"></param>
public sealed record class UserId(string Value);
