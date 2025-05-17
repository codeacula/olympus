using Olympus.Application.Common.Types;

namespace Olympus.Application.Ai.Errors;

public sealed record FailedToGetResponseError(string? Message) : OlympusError(nameof(FailedToGetResponseError), Message);
