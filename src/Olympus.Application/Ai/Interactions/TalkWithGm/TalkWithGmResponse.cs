using ProtoBuf;

namespace Olympus.Application.Ai.Interactions.TalkWithGm;

[ProtoContract]
public sealed record TalkWithGmResponse
{
  [ProtoMember(1)]
  public string Response { get; init; } = string.Empty;

  public TalkWithGmResponse(string response)
  {
    Response = response;
  }

  public override string ToString() => $"Response: {Response}";
}
