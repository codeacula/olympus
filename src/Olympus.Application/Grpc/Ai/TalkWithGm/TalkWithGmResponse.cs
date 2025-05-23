namespace Olympus.Application.Grpc.Ai.TalkWithGm;

[ProtoContract]
public sealed record TalkWithGmResponse
{
  [ProtoMember(1)]
  public required string Response { get; init; }

  public TalkWithGmResponse() { }

  public TalkWithGmResponse(string response)
  {
    Response = response;
  }
}
