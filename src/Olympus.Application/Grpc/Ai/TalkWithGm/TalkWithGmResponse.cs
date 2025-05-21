namespace Olympus.Application.Grpc.Ai.TalkWithGm;

[ProtoContract]
public sealed record TalkWithGmResponse
{
  [ProtoMember(1)]
  public string Response { get; init; } = string.Empty;

  public TalkWithGmResponse() { }

  public TalkWithGmResponse(string response)
  {
    Response = response;
  }
}
