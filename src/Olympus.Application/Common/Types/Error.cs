namespace Olympus.Application.Common.Types;

/// <summary>
/// Represents an error in the application layer.
/// </summary>
public sealed record class Error(string Code, string? Message = null);
