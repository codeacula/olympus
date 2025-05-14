namespace Olympus.Application.Common.Types;

/// <summary>
/// Represents an error in the application layer.
/// </summary>
/// <param name="Code"></param>
/// <param name="Message"></param>
public sealed record class Error(string Code, string? Message = null);
