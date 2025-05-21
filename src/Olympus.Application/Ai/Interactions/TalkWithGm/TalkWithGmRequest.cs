using ProtoBuf;

namespace Olympus.Application.Ai.Interactions.TalkWithGm;

[ProtoContract]
public sealed record TalkWithGmRequest : IRequest<TalkWithGmResponse>
{
  [ProtoMember(1)]
  public string InteractionText { get; init; } = string.Empty;

  public TalkWithGmRequest() { }

  public TalkWithGmRequest(string interactionText)
  {
    InteractionText = interactionText;
  }

  public override string ToString() => $"InteractionText: {InteractionText}";
}
