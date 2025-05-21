namespace Olympus.Application.Grpc.Ai.GreetGm;

[ProtoContract]
public sealed record GreetGmResponse
{
  [ProtoMember(1)]
  public string Response { get; init; } = string.Empty;

  public GreetGmResponse() { }

  public GreetGmResponse(string response)
  {
    Response = response;
  }
}
