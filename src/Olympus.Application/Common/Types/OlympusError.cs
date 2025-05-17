namespace Olympus.Application.Common.Types;

public abstract record class OlympusError(string Code, string? Message = null);
