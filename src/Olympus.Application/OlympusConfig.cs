namespace Olympus.Application;

public sealed record OlympusConfig
{
  public required string ApiHost { get; init; }
}
