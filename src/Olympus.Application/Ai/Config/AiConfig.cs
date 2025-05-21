namespace Olympus.Application.Ai.Config;

public sealed record AiConfig
{
  public required string ApiKey { get; init; }
  public required string Endpoint { get; init; }
  public required string ModelId { get; init; }
}
