namespace Olympus.Application.Ai.Interactions.TalkWithGm;

[ProtoContract]
public sealed record TalkWithGmResponse
{
  [ProtoMember(1)]
  public required string Value { get; init; }
}
