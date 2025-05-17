namespace Olympus.Application.Common.Types;

/// <summary>
/// Represents an error in the application layer.
/// </summary>
/// <param name="Code">What the error code is?</param>
/// <param name="Message">What the error message is?  </param>
public sealed record class OlympusError(string Code, string? Message = null);
