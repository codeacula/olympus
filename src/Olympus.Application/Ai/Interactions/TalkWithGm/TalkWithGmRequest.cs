namespace Olympus.Application.Ai.Interactions.TalkWithGm;

[ProtoContract]
public sealed record TalkWithGmRequest
{
  [ProtoMember(1)]
  public required string Value { get; init; }
}
