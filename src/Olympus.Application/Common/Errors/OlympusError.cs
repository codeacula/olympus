namespace Olympus.Application.Common.Errors;

/// <summary>
/// Base record for representing errors in the application layer.
/// </summary>
/// <param name="Message"></param>
public abstract record class OlympusError(string Message);
