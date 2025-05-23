namespace Olympus.Application.Grpc.Ai.TalkWithGm;

[ProtoContract]
public sealed record TalkWithGmRequest
{
  [ProtoMember(1)]
  public required string InteractionText { get; init; }

  public TalkWithGmRequest() { }

  public TalkWithGmRequest(string interactionText)
  {
    InteractionText = interactionText;
  }
}
