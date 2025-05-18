namespace Olympus.Application.Grpc;

public sealed record GrpcHost
{
  public required string Host { get; init; }
  public required int Port { get; init; }
  public bool UseHttps { get; init; } = true;

  public string ApiHost => $"{(UseHttps ? "https" : "http")}://{Host}:{Port}";
}
