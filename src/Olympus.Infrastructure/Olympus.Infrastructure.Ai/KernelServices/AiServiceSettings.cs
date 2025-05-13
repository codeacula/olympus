namespace Olympus.Infrastructure.Ai.KernelServices;

/// <summary>
/// Configuration settings for AI services (e.g., OpenAI).
/// </summary>
public sealed record class AiServiceSettings
{
    public string ApiKey { get; init; } = string.Empty;
    public string Endpoint { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
}
